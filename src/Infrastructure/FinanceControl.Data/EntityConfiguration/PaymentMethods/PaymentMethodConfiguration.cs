using FinanceControl.Contracts.Constants;
using FinanceControl.Domain.Entities.PaymentMethods;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceControl.Infrastructure.EntityConfiguration.PaymentMethods;

public class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
{
    public void Configure(EntityTypeBuilder<PaymentMethod> builder)
    {
        builder.ToTable("PaymentMethods", e => e.HasComment("Meios de pagamento cadastrados pelo utilizador"));

        builder.HasKey(p => p.PaymentMethodId);

        builder.Property(p => p.PaymentMethodId)
            .HasDefaultValueSql("NEWID()")
            .IsRequired();

        builder.Property(p => p.Name)
            .HasMaxLength(40)
            .IsRequired();

        builder.Property(p => p.Icon)
            .HasMaxLength(16)
            .IsRequired()
            .HasDefaultValue(PaymentMethodIcons.Default);

        builder.Property(p => p.Description)
            .HasMaxLength(200)
            .IsRequired(false);

        builder.Property(p => p.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(p => p.DateCreated)
            .IsRequired();

        builder.Property(p => p.UpdatedAt)
            .IsRequired(false);

        builder.Property(p => p.UserId)
            .IsRequired(false);

        builder.HasIndex(p => new { p.UserId, p.Name })
            .IsUnique()
            .HasDatabaseName("IX_PaymentMethods_UserId_Name");

        builder.HasOne(p => p.User)
            .WithMany(u => u.PaymentMethods)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
