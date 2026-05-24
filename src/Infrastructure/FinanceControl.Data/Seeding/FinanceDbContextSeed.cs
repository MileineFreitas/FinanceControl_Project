using FinanceControl.Contracts.Constants;
using FinanceControl.Contracts.Enumerators.Transactions;
using FinanceControl.Domain.Entities.Accounts;
using FinanceControl.Domain.Entities.Categories;
using FinanceControl.Domain.Entities.PaymentMethods;
using FinanceControl.Domain.Entities.Transactions;
using FinanceControl.Domain.Entities.Users;
using FinanceControl.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace FinanceControl.Infrastructure.Seeding;

/// <summary>
/// Dados iniciais para desenvolvimento. Executado após <see cref="DatabaseMigrationExtensions.ApplyMigrationsAndSeed"/>.
/// Idempotente: só insere o que ainda não existir.
/// </summary>
public static class FinanceDbContextSeed
{
    public static void EnsureDemoUserAccountAndCategories(FinanceDbContext db)
    {
        EnsurePaymentMethods(db);
        var userId = EnsureDemoUser(db);
        var accountId = EnsureDemoAccount(db, userId);
        EnsureDefaultCategories(db, userId);
        EnsureDemoTransactions(db, userId, accountId);
    }

    private static void EnsurePaymentMethods(FinanceDbContext db)
    {
        if (db.PaymentMethods.Any()) return;

        var utc = DateTimeOffset.UtcNow;
        db.PaymentMethods.AddRange(
            new PaymentMethod
            {
                PaymentMethodId = PaymentMethodSeedIds.Debito,
                Name = "Débito",
                Icon = "💳",
                IsActive = true,
                DateCreated = utc
            },
            new PaymentMethod
            {
                PaymentMethodId = PaymentMethodSeedIds.Credito,
                Name = "Crédito",
                Icon = "💳",
                IsActive = true,
                DateCreated = utc
            },
            new PaymentMethod
            {
                PaymentMethodId = PaymentMethodSeedIds.Dinheiro,
                Name = "Dinheiro",
                Icon = "💵",
                IsActive = true,
                DateCreated = utc
            },
            new PaymentMethod
            {
                Name = "PIX",
                Icon = "📱",
                Description = "Pagamento instantâneo",
                IsActive = true,
                DateCreated = utc
            });
        db.SaveChanges();
    }

    private static Guid EnsureDemoUser(FinanceDbContext db)
    {
        var existing = db.Users.OrderBy(u => u.UserId).FirstOrDefault();
        if (existing != null)
            return existing.UserId;

        var user = new User
        {
            UserName = "Usuário Demo",
            UserEmail = "demo@financecontrol.local",
            Password = "demo123",
            IsActive = true,
            DateCreated = DateTime.UtcNow
        };
        db.Users.Add(user);
        db.SaveChanges();
        return user.UserId;
    }

    private static Guid EnsureDemoAccount(FinanceDbContext db, Guid userId)
    {
        var acc = db.Accounts.FirstOrDefault(a => a.AccountId == SeedIds.DefaultAccount);
        if (acc == null)
        {
            acc = new Account
            {
                AccountId = SeedIds.DefaultAccount,
                Name = "Conta Principal",
                InitialBalance = 0,
                CurrentBalance = 0,
                CreatedAt = DateTime.UtcNow,
                UserId = userId,
                IsActive = true
            };
            db.Accounts.Add(acc);
            db.SaveChanges();
            return acc.AccountId;
        }

        if (acc.UserId == null)
        {
            acc.UserId = userId;
            db.SaveChanges();
        }

        return acc.AccountId;
    }

    private static void EnsureDefaultCategories(FinanceDbContext db, Guid userId)
    {
        if (db.Categories.Any()) return;

        var utc = DateTimeOffset.UtcNow;
        db.Categories.AddRange(
            new Category { CategoryName = "Salário", Description = "Receitas fixas e CLT", Icon = "💼", DateCreated = utc, UserId = userId, IsActive = true },
            new Category { CategoryName = "Investimentos", Description = "Dividendos e rendimentos", Icon = "📈", DateCreated = utc, UserId = userId, IsActive = true },
            new Category { CategoryName = "Freelance", Description = "Trabalhos extras", Icon = "🚀", DateCreated = utc, UserId = userId, IsActive = true },
            new Category { CategoryName = "Moradia", Description = "Aluguel, condomínio, IPTU", Icon = "🏠", DateCreated = utc, UserId = userId, IsActive = true },
            new Category { CategoryName = "Alimentação", Description = "Mercado e refeições", Icon = "🛒", DateCreated = utc, UserId = userId, IsActive = true },
            new Category { CategoryName = "Transporte", Description = "Combustível, apps, transporte público", Icon = "🚗", DateCreated = utc, UserId = userId, IsActive = true },
            new Category { CategoryName = "Saúde", Description = "Plano, farmácia, consultas", Icon = "🏥", DateCreated = utc, UserId = userId, IsActive = true },
            new Category { CategoryName = "Lazer", Description = "Streaming, viagens, hobbies", Icon = "🎮", DateCreated = utc, UserId = userId, IsActive = true },
            new Category { CategoryName = "Educação", Description = "Cursos e materiais", Icon = "🎓", DateCreated = utc, UserId = userId, IsActive = true });

        db.SaveChanges();
    }

    private static void EnsureDemoTransactions(FinanceDbContext db, Guid userId, Guid accountId)
    {
        if (db.Transactions.Any()) return;

        var cats = db.Categories.AsNoTracking().ToList();
        if (cats.Count == 0) return;

        var utc = DateTimeOffset.UtcNow;
        var today = utc.Date;

        Guid Cat(string name)
        {
            var c = cats.FirstOrDefault(x => string.Equals(x.CategoryName, name, StringComparison.OrdinalIgnoreCase));
            return c?.CategoryId ?? cats[0].CategoryId;
        }

        void Add(string desc, decimal val, TransactionTypeKind tipo, string categoryName, DateTime when, PaymentKind meio)
        {
            db.Transactions.Add(new Transaction
            {
                TransactionDescription = desc,
                TransactionValue = val,
                Date = when,
                TransactionTypeKind = tipo,
                PaymentKind = meio,
                CategoryId = Cat(categoryName),
                AccountId = accountId,
                UserId = userId,
                CreatedAt = utc,
                UpdatedAt = utc
            });
        }

        Add("Salário CLT", 9200m, TransactionTypeKind.Receita, "Salário", today.AddDays(-3), PaymentKind.Cash);
        Add("Freelance design", 4500m, TransactionTypeKind.Receita, "Freelance", today.AddDays(-11), PaymentKind.Debit);
        Add("Dividendos ITUB4", 340m, TransactionTypeKind.Receita, "Investimentos", today.AddDays(-2), PaymentKind.Credit);
        Add("PIX recebido", 800m, TransactionTypeKind.Receita, "Freelance", today.AddDays(-7), PaymentKind.Debit);
        Add("Aluguel", 2400m, TransactionTypeKind.Despesa, "Moradia", today.AddDays(-5), PaymentKind.Credit);
        Add("Supermercado", 412.55m, TransactionTypeKind.Despesa, "Alimentação", today.AddDays(-1), PaymentKind.Cash);
        Add("Netflix", 55.90m, TransactionTypeKind.Despesa, "Lazer", today.AddDays(-10), PaymentKind.Credit);
        Add("Combustível", 280m, TransactionTypeKind.Despesa, "Transporte", today.AddDays(-4), PaymentKind.Debit);
        Add("Farmácia", 89.50m, TransactionTypeKind.Despesa, "Saúde", today.AddDays(-6), PaymentKind.Cash);
        Add("Curso online", 199m, TransactionTypeKind.Despesa, "Educação", today.AddDays(-14), PaymentKind.Credit);
        Add("Restaurante", 165m, TransactionTypeKind.Despesa, "Lazer", today.AddDays(-2), PaymentKind.Debit);

        db.SaveChanges();

        var account = db.Accounts.FirstOrDefault(a => a.AccountId == accountId);
        if (account == null) return;

        decimal balance = 0;
        foreach (var t in db.Transactions)
            balance += t.TransactionTypeKind == TransactionTypeKind.Receita ? t.TransactionValue : -t.TransactionValue;

        account.CurrentBalance = balance;
        db.SaveChanges();
    }
}
