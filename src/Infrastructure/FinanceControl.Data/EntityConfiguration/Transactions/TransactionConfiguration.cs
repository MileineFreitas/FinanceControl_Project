using FinanceControl.Domain.Entities.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceControl.Infrastructure.EntityConfiguration.Transactions;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("Transactions");

        builder.HasKey(t => t.TransactionId);
        builder.Property(t => t.TransactionId)
            .HasDefaultValueSql("NEWID()")
            .IsRequired();

        builder.Property(t => t.TransactionDescription)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(t => t.TransactionValue)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(t => t.Date)
            .IsRequired();

        builder.Property(t => t.TransactionTypeKind)
            .HasConversion<int>()
            .HasColumnName("FlowDirection")
            .IsRequired();

        builder.Property(t => t.PaymentMethodId)
            .IsRequired();

        builder.Property(t => t.CategoryId)
            .IsRequired();

        builder.Property(t => t.AccountId)
            .IsRequired(false);

        builder.Property(t => t.UserId)
            .IsRequired();

        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.Property(t => t.UpdatedAt)
            .IsRequired(false);

        builder.HasOne(t => t.PaymentMethod)
            .WithMany()
            .HasForeignKey(t => t.PaymentMethodId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Account)
            .WithMany(a => a.Transactions)
            .HasForeignKey(t => t.AccountId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(t => t.Category)
            .WithMany(c => c.Transactions)
            .HasForeignKey(t => t.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.User)
            .WithMany(u => u.Transactions)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
