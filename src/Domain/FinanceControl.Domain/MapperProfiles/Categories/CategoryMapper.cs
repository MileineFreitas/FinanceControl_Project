using FinanceControl.Contracts.Constants;
using FinanceControl.Contracts.Dtos.Categories;
using FinanceControl.Domain.Entities.Categories;

namespace FinanceControl.Domain.MapperProfiles.Categories;

public static class CategoryMapper
{
    public static CategoryDto ToDto(Category entity) =>
        new()
        {
            CategoryId = entity.CategoryId,
            CategoryName = entity.CategoryName,
            Description = entity.Description,
            Icon = CategoryIcons.Normalize(entity.Icon),
            DateCreated = entity.DateCreated,
            UpdatedAt = entity.UpdatedAt,
            UserId = entity.UserId
        };

    public static Category ToEntity(CategoryRegisterDto dto, int? userId) =>
        new()
        {
            CategoryName = dto.CategoryName.Trim(),
            Description = dto.CategoryDescription?.Trim(),
            Icon = CategoryIcons.Normalize(dto.Icon),
            DateCreated = DateTime.UtcNow,
            UserId = userId
        };

    public static void ApplyUpdate(Category entity, CategoryUpdateDto dto)
    {
        entity.CategoryName = dto.CategoryName.Trim();
        entity.Description = dto.Description?.Trim();
        entity.Icon = CategoryIcons.Normalize(dto.Icon);
        entity.UpdatedAt = DateTime.UtcNow;
    }
}
