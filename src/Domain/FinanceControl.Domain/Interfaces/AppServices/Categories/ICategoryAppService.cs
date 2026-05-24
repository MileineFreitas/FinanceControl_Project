using FinanceControl.Contracts.Dtos.Categories;
using FinanceControl.Contracts.Dtos.Common;
using FinanceControl.Contracts.Filters;

namespace FinanceControl.Domain.Interfaces.AppServices.Categories;

public interface ICategoryAppService
{
    Task<DataResultDto<CategoryDto>> FilterAsync(DataFilterDto filter);

    Task<CategoryDto?> GetByIdAsync(int id);

    Task<CategoryDto> CreateAsync(CategoryRegisterDto dto);

    Task<CategoryDto?> UpdateAsync(CategoryUpdateDto dto);

    Task<bool> DeleteAsync(int id);
}
