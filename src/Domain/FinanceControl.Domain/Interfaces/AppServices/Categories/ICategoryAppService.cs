using FinanceControl.Contracts.Dtos.Categories;
using FinanceControl.Contracts.Dtos.Common;
using FinanceControl.Contracts.Filters;

namespace FinanceControl.Domain.Interfaces.AppServices.Categories;

public interface ICategoryAppService
{
    Task<DataResultDto<CategoryDto>> FilterAsync(DataFilterDto filter, CancellationToken cancellationToken = default);

    Task<CategoryDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<CategoryDto> CreateAsync(CategoryRegisterDto dto, CancellationToken cancellationToken = default);

    Task<CategoryDto?> UpdateAsync(CategoryUpdateDto dto, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
