using FinanceControl.Contracts.Constants;
using FinanceControl.Domain.Entities.Categories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceControl.Infrastructure.EntityConfiguration.Categories;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories", e => e.HasComment("Tabela de categorias de transações"));

        builder.HasKey(c => c.CategoryId);
        builder.Property(c => c.CategoryId)
            .HasDefaultValueSql("(NEWID())")
            .IsRequired()
            .HasComment("Identificador único da categoria (GUID)");

        builder.Property(c => c.CategoryName)
            .HasMaxLength(40)
            .IsRequired()
            .HasComment("Nome da categoria");

        builder.Property(c => c.Description)
            .HasMaxLength(255)
            .IsRequired(false)
            .HasComment("Descrição opcional da categoria");

        builder.Property(c => c.Icon)
            .HasMaxLength(16)
            .IsRequired()
            .HasDefaultValue(CategoryIcons.Default)
            .HasComment("Ícone representativo da categoria");

        builder.Property(c => c.DateCreated)
            .IsRequired()
            .HasComment("Data de criação do registro");

        builder.Property(c => c.UpdatedAt)
            .IsRequired(false)
            .HasComment("Data da última atualização");

        builder.Property(c => c.UserId)
            .IsRequired(false)
            .HasComment("Referência ao usuário proprietário da categoria");

        builder.HasOne(c => c.User)
            .WithMany(u => u.Categories)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Transactions)
            .WithOne(t => t.Category)
            .HasForeignKey("CategoryId")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
