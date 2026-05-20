using FinanceControl.Contracts.Enumerators.Transactions;
using FinanceControl.Domain.Entities.TransactionTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceControl.Infrastructure.EntityConfiguration.TransactionTypes;

public class TransactionTypeConfiguration : IEntityTypeConfiguration<TransactionTypeDefinition>
{
    public void Configure(EntityTypeBuilder<TransactionTypeDefinition> builder)
    {
        builder.ToTable("TransactionTypes");
        builder.HasKey(t => t.TransactionTypeId);
        builder.Property(t => t.Name).HasMaxLength(40).IsRequired();
        builder.Property(t => t.Code).HasMaxLength(20).IsRequired();
        builder.Property(t => t.Icon).HasMaxLength(16).IsRequired().HasDefaultValue("💳");
        builder.HasIndex(t => t.Code).IsUnique();
        builder.Property(t => t.Description).HasMaxLength(200);
        builder.HasOne(t => t.User)
            .WithMany()
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        var seedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        builder.HasData(
            new TransactionTypeDefinition
            {
                TransactionTypeId = 1,
                Name = "Débito",
                Code = "DEBITO",
                Icon = "💳",
                PaymentKind = PaymentKind.Debit,
                IsSystem = true,
                IsActive = true,
                CreatedAt = seedAt
            },
            new TransactionTypeDefinition
            {
                TransactionTypeId = 2,
                Name = "Crédito",
                Code = "CREDITO",
                Icon = "💳",
                PaymentKind = PaymentKind.Credit,
                IsSystem = true,
                IsActive = true,
                CreatedAt = seedAt
            },
            new TransactionTypeDefinition
            {
                TransactionTypeId = 3,
                Name = "Dinheiro",
                Code = "DINHEIRO",
                Icon = "💵",
                PaymentKind = PaymentKind.Cash,
                IsSystem = true,
                IsActive = true,
                CreatedAt = seedAt
            });
    }
}
