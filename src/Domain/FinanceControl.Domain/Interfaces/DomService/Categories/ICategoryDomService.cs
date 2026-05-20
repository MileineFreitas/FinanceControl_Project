using FinanceControl.Contracts.Dtos.Categories;
using FinanceControl.Domain.Entities.Categories;

namespace FinanceControl.Domain.Interfaces.DomService.Categories;

public interface ICategoryDomService
{
    Category CreateFromRegister(CategoryRegisterDto dto, int? userId);

    void ApplyUpdate(Category entity, CategoryUpdateDto dto);
}
