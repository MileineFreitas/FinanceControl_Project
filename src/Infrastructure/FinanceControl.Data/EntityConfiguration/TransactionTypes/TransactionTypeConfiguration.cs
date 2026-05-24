using FinanceControl.Contracts.Constants;
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

        builder.Property(t => t.TransactionTypeId)
            .HasDefaultValueSql("(NEWID())")
            .IsRequired();

        builder.Property(t => t.Name)
            .HasMaxLength(40)
            .IsRequired();

        builder.Property(t => t.Icon)
            .HasMaxLength(16)
            .IsRequired()
            .HasDefaultValue(PaymentMethodIcons.Default);

        builder.Property(t => t.Description)
            .HasMaxLength(200)
            .IsRequired(false);

        builder.Property(t => t.IsSystem)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(t => t.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
    }
}
