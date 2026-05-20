using FinanceControl.Contracts.Dtos.Categories;
using FinanceControl.Contracts.Dtos.Common;
using FinanceControl.Contracts.Filters;

namespace FinanceControl.Client.Services.Interfaces.Categories;

public interface ICategoryCliService
{
    Task<DataResultDto<CategoryDto>?> ListAsync(DataFilterDto? filter = null, CancellationToken cancellationToken = default);

    Task<CategoryDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<HttpResponseMessage> CreateAsync(CategoryRegisterDto dto, CancellationToken cancellationToken = default);

    Task<HttpResponseMessage> UpdateAsync(int id, CategoryUpdateDto dto, CancellationToken cancellationToken = default);

    Task<HttpResponseMessage> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
