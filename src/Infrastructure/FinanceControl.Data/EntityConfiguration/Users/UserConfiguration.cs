using FinanceControl.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceControl.Infrastructure.EntityConfiguration.Users;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(u => u.UserId);
        builder.Property(u => u.UserId)
            .HasDefaultValueSql("NEWID()")
            .IsRequired();

        builder.Property(u => u.UserName).HasMaxLength(100).IsRequired();
        builder.Property(u => u.UserEmail).HasMaxLength(200).IsRequired();
        builder.Property(u => u.Password).HasMaxLength(20).IsRequired();
        builder.Property(u => u.ProfilePhoto).HasColumnType("nvarchar(max)").IsRequired(false);
        builder.Property(u => u.IsActive).IsRequired().HasDefaultValue(true);
        builder.Property(u => u.SecurityStamp)
            .IsRequired()
            .HasDefaultValueSql("NEWID()");
        builder.Property(u => u.DateCreated).IsRequired();
        builder.Property(u => u.Currency).HasMaxLength(3).IsRequired().HasDefaultValue("BRL");
        builder.Property(u => u.Language).HasMaxLength(10).IsRequired().HasDefaultValue("pt-BR");
        builder.Property(u => u.DateFormat).HasMaxLength(20).IsRequired().HasDefaultValue("dd/MM/yyyy");
        builder.Property(u => u.FinancialMonthStartDay).IsRequired().HasDefaultValue(1);
    }
}
