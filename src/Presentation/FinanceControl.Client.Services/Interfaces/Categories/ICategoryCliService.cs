using FinanceControl.Contracts.Dtos.Categories;
using FinanceControl.Contracts.Dtos.Common;
using FinanceControl.Contracts.Filters;

namespace FinanceControl.Client.Services.Interfaces.Categories;

public interface ICategoryCliService
{
    Task<DataResultDto<CategoryDto>?> ListAsync(DataFilterDto? filter = null, bool includeInactive = true);

    Task<CategoryDto?> GetByIdAsync(Guid id);

    Task<HttpResponseMessage> CreateAsync(CategoryRegisterDto dto);

    Task<HttpResponseMessage> UpdateAsync(Guid id, CategoryUpdateDto dto);

    Task<HttpResponseMessage> DeleteAsync(Guid id);
}
