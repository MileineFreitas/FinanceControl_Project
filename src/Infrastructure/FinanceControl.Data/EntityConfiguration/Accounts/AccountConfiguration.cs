using FinanceControl.Domain.Entities.Accounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceControl.Infrastructure.EntityConfiguration.Accounts;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("Accounts");
        builder.HasKey(a => a.AccountId);
        builder.Property(a => a.AccountId)
            .HasDefaultValueSql("NEWID()")
            .IsRequired();
        builder.Property(a => a.Name).HasMaxLength(120).IsRequired();
        builder.Property(a => a.InitialBalance).HasPrecision(18, 2);
        builder.Property(a => a.CurrentBalance).HasPrecision(18, 2);
        builder.Property(a => a.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasOne(a => a.User)
            .WithMany(u => u.Accounts)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
