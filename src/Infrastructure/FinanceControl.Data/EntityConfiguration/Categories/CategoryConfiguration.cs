using FinanceControl.Contracts.Constants;
using FinanceControl.Domain.Entities.Categories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceControl.Infrastructure.EntityConfiguration.Categories;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");
        builder.HasKey(c => c.CategoryId);
        builder.Property(c => c.CategoryName).HasMaxLength(40).IsRequired();
        builder.Property(c => c.Icon).HasMaxLength(16).IsRequired().HasDefaultValue(CategoryIcons.Default);
    }
}
