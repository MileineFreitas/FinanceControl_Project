using FinanceControl.Contracts.Dtos.Categories;
using FinanceControl.Domain.Entities.Categories;
using FinanceControl.Domain.Interfaces.DomService.Categories;
using FinanceControl.Domain.MapperProfiles.Categories;

namespace FinanceControl.Domain.Services.Categories;

public class CategoryDomService : ICategoryDomService
{
    public Category CreateFromRegister(CategoryRegisterDto dto, int? userId)
    {
        if (string.IsNullOrWhiteSpace(dto.CategoryName) || dto.CategoryName.Trim().Length < 2)
            throw new ArgumentException("Nome da categoria deve ter pelo menos 2 caracteres.");

        return CategoryMapper.ToEntity(dto, userId);
    }

    public void ApplyUpdate(Category entity, CategoryUpdateDto dto) =>
        CategoryMapper.ApplyUpdate(entity, dto);
}
