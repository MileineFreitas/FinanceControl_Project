using FinanceControl.Domain.Entities.Categories;
using FinanceControl.Domain.Entities.Transactions;
using FinanceControl.Domain.Entities.Users;
using FinanceControl.Domain.Enums;
using FinanceControl.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace FinanceControl.Infrastructure.Seeding;

/// <summary>
/// Garante utilizador demo, conta principal, categorias e — quando vazio —
/// lançamentos fictícios para telas e dashboard parecerem populados.
/// </summary>
public static class FinanceDbContextSeed
{
    public static void EnsureDemoUserAccountAndCategories(FinanceDbContext db)
    {
        EnsureDemoUserAndAccount(db);
        EnsureDefaultCategories(db);
        EnsureDemoTransactions(db);
    }

    private static void EnsureDemoUserAndAccount(FinanceDbContext db)
    {
        if (!db.Users.Any())
        {
            db.Users.Add(new User
            {
                UserName = "Usuário demo",
                UserEmail = "demo@financecontrol.local",
                Password = "demo",
                UserType = UserType.User,
                DateCreated = DateTime.UtcNow
            });
            db.SaveChanges();
        }

        var userId = db.Users.OrderBy(u => u.UserId).First().UserId;

        var acc = db.Accounts.FirstOrDefault(a => a.AccountId == 1);
        if (acc != null && acc.UserId == null)
        {
            acc.UserId = userId;
            db.SaveChanges();
        }
    }

    private static void EnsureDefaultCategories(FinanceDbContext db)
    {
        if (db.Categories.Any())
            return;

        var userId = db.Users.OrderBy(u => u.UserId).Select(u => (int?)u.UserId).FirstOrDefault();
        var utc = DateTime.UtcNow;

        db.Categories.AddRange(
            new Category { CategoryName = "Salário", Description = "Receitas fixas", TransactionTypeId = 1, DateCreated = utc, UserId = userId },
            new Category { CategoryName = "Investimentos", Description = "Rendimentos", TransactionTypeId = 1, DateCreated = utc, UserId = userId },
            new Category { CategoryName = "Moradia", Description = "Aluguel e condomínio", TransactionTypeId = 2, DateCreated = utc, UserId = userId },
            new Category { CategoryName = "Alimentação", Description = "Mercado e refeições", TransactionTypeId = 2, DateCreated = utc, UserId = userId },
            new Category { CategoryName = "Lazer", Description = "Entretenimento", TransactionTypeId = 2, DateCreated = utc, UserId = userId });

        db.SaveChanges();
    }

    /// <summary>Lançamentos fictícios (apenas se <c>Transactions</c> estiver vazio).</summary>
    private static void EnsureDemoTransactions(FinanceDbContext db)
    {
        if (db.Transactions.Any())
            return;

        var cats = db.Categories.AsNoTracking().ToList();
        if (cats.Count == 0)
            return;

        var userId = db.Users.OrderBy(u => u.UserId).First().UserId;
        const int accountId = 1;

        int Cat(string name)
        {
            var c = cats.FirstOrDefault(x =>
                string.Equals(x.CategoryName, name, StringComparison.OrdinalIgnoreCase));
            return c?.CategoryId ?? cats[0].CategoryId;
        }

        var utc = DateTime.UtcNow;
        var today = utc.Date;

        void Add(string desc, decimal val, int transactionTypeId, string categoryName, DateTime when)
        {
            db.Transactions.Add(new Transaction
            {
                TransactionDescription = desc,
                TransactionValue = val,
                Date = when,
                TransactionTypeId = transactionTypeId,
                CategoryId = Cat(categoryName),
                AccountId = accountId,
                UserId = userId,
                Status = TransactionStatus.Pago,
                CreatedAt = utc,
                UpdatedAt = utc
            });
        }

        // Mês corrente e semanas anteriores — valores em R$ plausíveis
        Add("Salário CLT — Tech Solutions Brasil", 9200m, 1, "Salário", today.AddDays(-3).AddHours(8).AddMinutes(30));
        Add("Freelance — Consultoria financeira (NF 8842)", 4500m, 1, "Salário", today.AddDays(-11).AddHours(14));
        Add("Dividendos PETR4 — corretora", 340m, 1, "Investimentos", today.AddDays(-2).AddHours(16).AddMinutes(20));
        Add("Resgate CDB liquidez diária", 1200m, 1, "Investimentos", today.AddDays(-18).AddHours(10));

        Add("Aluguel apartamento", 2400m, 2, "Moradia", today.AddDays(-5).AddHours(9));
        Add("Condomínio + fundo reserva", 680m, 2, "Moradia", today.AddDays(-5).AddHours(9).AddMinutes(15));
        Add("Energia elétrica — Enel", 189.90m, 2, "Moradia", today.AddDays(-8));
        Add("Internet fibra 500 Mb", 109.90m, 2, "Moradia", today.AddDays(-12));

        Add("Supermercado Carrefour", 412.55m, 2, "Alimentação", today.AddDays(-1).AddHours(19).AddMinutes(22));
        Add("iFood — restaurantes", 67.40m, 2, "Alimentação", today.AddDays(-4).AddHours(20));
        Add("Padaria e café da manhã", 28.50m, 2, "Alimentação", today.AddDays(-6).AddHours(7).AddMinutes(40));

        Add("Netflix assinatura", 55.90m, 2, "Lazer", today.AddDays(-10));
        Add("Spotify Premium", 24.90m, 2, "Lazer", today.AddDays(-15));
        Add("Ingresso cinema — Shopping", 52m, 2, "Lazer", today.AddDays(-9).AddHours(21));

        Add("Apple Store — capa iPhone", 129m, 2, "Lazer", today.AddDays(-14).AddHours(15));
        Add("Uber — aeroporto", 87m, 2, "Alimentação", today.AddDays(-20).AddHours(6));
        Add("Farmácia Drogasil", 156.30m, 2, "Moradia", today.AddDays(-7));

        db.SaveChanges();

        var account = db.Accounts.First(a => a.AccountId == accountId);
        decimal balance = 0;
        foreach (var t in db.Transactions.Where(x => x.Status == TransactionStatus.Pago))
            balance += t.TransactionTypeId == 1 ? t.TransactionValue : -t.TransactionValue;

        account.CurrentBalance = balance;
        db.SaveChanges();
    }
}
