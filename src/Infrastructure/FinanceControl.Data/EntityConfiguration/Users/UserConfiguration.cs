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

        builder.Property(u => u.UserName).HasMaxLength(100).IsRequired();
        builder.Property(u => u.UserEmail).HasMaxLength(200).IsRequired();
        builder.Property(u => u.Password).HasMaxLength(20).IsRequired();
        builder.Property(u => u.ProfilePhoto).HasMaxLength(500).IsRequired(false);
        builder.Property(u => u.IsActive).IsRequired().HasDefaultValue(true);
        builder.Property(u => u.DateCreated).IsRequired();
    }
}
