using FinanceControl.Domain.Entities.Accounts;
using FinanceControl.Domain.Entities.Categories;
using FinanceControl.Domain.Entities.PaymentMethods;
using FinanceControl.Domain.Entities.TransactionTypes;
using FinanceControl.Domain.Entities.Transactions;
using FinanceControl.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace FinanceControl.Infrastructure.Contexts;

public class FinanceDbContext : DbContext
{
    public FinanceDbContext(DbContextOptions<FinanceDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Transaction> Transactions { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Account> Accounts { get; set; } = null!;
    public DbSet<TransactionTypeDefinition> TransactionTypes { get; set; } = null!;
    public DbSet<PaymentMethod> PaymentMethods { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TransactionTypeDefinition>(e =>
        {
            e.HasKey(x => x.TransactionTypeId);
            e.HasData(
                new TransactionTypeDefinition { TransactionTypeId = 1, Name = "RECEITA" },
                new TransactionTypeDefinition { TransactionTypeId = 2, Name = "DESPESA" });
        });

        modelBuilder.Entity<Account>(e =>
        {
            e.Property(a => a.InitialBalance).HasPrecision(18, 2);
            e.Property(a => a.CurrentBalance).HasPrecision(18, 2);
            e.HasOne(a => a.User)
                .WithMany(u => u.Accounts)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Category>(e =>
        {
            e.HasOne(c => c.TransactionTypeDefinition)
                .WithMany()
                .HasForeignKey(c => c.TransactionTypeId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Transaction>(e =>
        {
            e.Property(t => t.TransactionValue).HasPrecision(18, 2);
            e.HasOne(t => t.TransactionTypeDefinition)
                .WithMany()
                .HasForeignKey(t => t.TransactionTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(t => t.Account)
                .WithMany(a => a.Transactions)
                .HasForeignKey(t => t.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(t => t.Category)
                .WithMany(c => c.Transactions)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(t => t.User)
                .WithMany(u => u.Transactions)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        var seedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        modelBuilder.Entity<Account>().HasData(
            new Account
            {
                AccountId = 1,
                Name = "Principal",
                InitialBalance = 0,
                CurrentBalance = 0,
                CreatedAt = seedAt,
                UserId = null
            });
    }
}
