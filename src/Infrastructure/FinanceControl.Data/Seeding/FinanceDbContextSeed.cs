using FinanceControl.Contracts.Constants;
using FinanceControl.Contracts.Enumerators.Transactions;
using FinanceControl.Domain.Entities.Accounts;
using FinanceControl.Domain.Entities.Categories;
using FinanceControl.Domain.Entities.TransactionTypes;
using FinanceControl.Domain.Entities.Transactions;
using FinanceControl.Domain.Entities.Users;
using FinanceControl.Domain.Enums;
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
        EnsureTransactionTypes(db);
        var userId = EnsureDemoUser(db);
        EnsureDemoAccount(db, userId);
        EnsureDefaultCategories(db, userId);
        EnsureDemoTransactions(db, userId);
    }

    private static void EnsureTransactionTypes(FinanceDbContext db)
    {
        if (db.TransactionTypes.Any())
        {
            MigrateLegacyReceitaDespesaTypes(db);
            EnsurePixPaymentType(db);
            return;
        }

        var utc = DateTime.UtcNow;
        db.TransactionTypes.AddRange(
            new TransactionTypeDefinition
            {
                TransactionTypeId = 1, Name = "Débito", Code = "DEBITO", Icon = "💳",
                PaymentKind = PaymentKind.Debit, IsSystem = true, IsActive = true, CreatedAt = utc
            },
            new TransactionTypeDefinition
            {
                TransactionTypeId = 2, Name = "Crédito", Code = "CREDITO", Icon = "💳",
                PaymentKind = PaymentKind.Credit, IsSystem = true, IsActive = true, CreatedAt = utc
            },
            new TransactionTypeDefinition
            {
                TransactionTypeId = 3, Name = "Dinheiro", Code = "DINHEIRO", Icon = "💵",
                PaymentKind = PaymentKind.Cash, IsSystem = true, IsActive = true, CreatedAt = utc
            },
            new TransactionTypeDefinition
            {
                TransactionTypeId = 4, Name = "PIX", Code = "PIX", Icon = "📱",
                PaymentKind = null, IsSystem = false, IsActive = true, CreatedAt = utc
            });
        db.SaveChanges();
    }

    private static void EnsurePixPaymentType(FinanceDbContext db)
    {
        if (db.TransactionTypes.Any(t => t.Code == "PIX")) return;

        db.TransactionTypes.Add(new TransactionTypeDefinition
        {
            Name = "PIX",
            Code = "PIX",
            Icon = "📱",
            PaymentKind = null,
            IsSystem = false,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });
        db.SaveChanges();
    }

    private static void MigrateLegacyReceitaDespesaTypes(FinanceDbContext db)
    {
        var receita = db.TransactionTypes.FirstOrDefault(t => t.Name == "RECEITA");
        var despesa = db.TransactionTypes.FirstOrDefault(t => t.Name == "DESPESA");
        if (receita == null && despesa == null) return;

        var utc = DateTime.UtcNow;
        if (receita != null)
        {
            receita.Name = "Débito";
            receita.Code = "DEBITO";
            receita.PaymentKind = PaymentKind.Debit;
            receita.IsSystem = true;
            receita.IsActive = true;
            receita.UpdatedAt = utc;
        }

        if (despesa != null)
        {
            despesa.Name = "Crédito";
            despesa.Code = "CREDITO";
            despesa.PaymentKind = PaymentKind.Credit;
            despesa.IsSystem = true;
            despesa.IsActive = true;
            despesa.UpdatedAt = utc;
        }

        if (!db.TransactionTypes.Any(t => t.Code == "DINHEIRO"))
        {
            db.TransactionTypes.Add(new TransactionTypeDefinition
            {
                Name = "Dinheiro",
                Code = "DINHEIRO",
                Icon = "💵",
                PaymentKind = PaymentKind.Cash,
                IsSystem = true,
                IsActive = true,
                CreatedAt = utc
            });
        }

        db.SaveChanges();
    }

    private static int EnsureDemoUser(FinanceDbContext db)
    {
        var existing = db.Users.OrderBy(u => u.UserId).FirstOrDefault();
        if (existing != null)
            return existing.UserId;

        var user = new User
        {
            UserName = "Usuário Demo",
            UserEmail = "demo@financecontrol.local",
            Password = "demo123",
            UserType = UserType.User,
            DateCreated = DateTime.UtcNow
        };
        db.Users.Add(user);
        db.SaveChanges();
        return user.UserId;
    }

    private static void EnsureDemoAccount(FinanceDbContext db, int userId)
    {
        var acc = db.Accounts.FirstOrDefault(a => a.AccountId == 1);
        if (acc == null)
        {
            db.Accounts.Add(new Account
            {
                AccountId = 1,
                Name = "Conta Principal",
                InitialBalance = 0,
                CurrentBalance = 0,
                CreatedAt = DateTime.UtcNow,
                UserId = userId
            });
            db.SaveChanges();
            return;
        }

        if (acc.UserId == null)
        {
            acc.UserId = userId;
            db.SaveChanges();
        }
    }

    private static void EnsureDefaultCategories(FinanceDbContext db, int userId)
    {
        if (db.Categories.Any()) return;

        var utc = DateTime.UtcNow;
        db.Categories.AddRange(
            new Category { CategoryName = "Salário", Description = "Receitas fixas e CLT", Icon = "💼", DateCreated = utc, UserId = userId },
            new Category { CategoryName = "Investimentos", Description = "Dividendos e rendimentos", Icon = "📈", DateCreated = utc, UserId = userId },
            new Category { CategoryName = "Freelance", Description = "Trabalhos extras", Icon = "🚀", DateCreated = utc, UserId = userId },
            new Category { CategoryName = "Moradia", Description = "Aluguel, condomínio, IPTU", Icon = "🏠", DateCreated = utc, UserId = userId },
            new Category { CategoryName = "Alimentação", Description = "Mercado e refeições", Icon = "🛒", DateCreated = utc, UserId = userId },
            new Category { CategoryName = "Transporte", Description = "Combustível, apps, transporte público", Icon = "🚗", DateCreated = utc, UserId = userId },
            new Category { CategoryName = "Saúde", Description = "Plano, farmácia, consultas", Icon = "🏥", DateCreated = utc, UserId = userId },
            new Category { CategoryName = "Lazer", Description = "Streaming, viagens, hobbies", Icon = "🎮", DateCreated = utc, UserId = userId },
            new Category { CategoryName = "Educação", Description = "Cursos e materiais", Icon = "🎓", DateCreated = utc, UserId = userId });

        db.SaveChanges();
    }

    private static void EnsureDemoTransactions(FinanceDbContext db, int userId)
    {
        if (db.Transactions.Any()) return;

        var cats = db.Categories.AsNoTracking().ToList();
        if (cats.Count == 0) return;

        const int accountId = 1;
        var utc = DateTime.UtcNow;
        var today = utc.Date;

        int Cat(string name)
        {
            var c = cats.FirstOrDefault(x => string.Equals(x.CategoryName, name, StringComparison.OrdinalIgnoreCase));
            return c?.CategoryId ?? cats[0].CategoryId;
        }

        void Add(string desc, decimal val, TransactionTypeKind tipo, string categoryName, DateTime when, PaymentKind? meio = null)
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
                Status = TransactionStatus.Pago,
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
        foreach (var t in db.Transactions.Where(x => x.Status == TransactionStatus.Pago))
            balance += t.TransactionTypeKind == TransactionTypeKind.Receita ? t.TransactionValue : -t.TransactionValue;

        account.CurrentBalance = balance;
        db.SaveChanges();
    }
}
