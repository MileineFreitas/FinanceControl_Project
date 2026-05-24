using FinanceControl.Contracts.Dtos.Categories;
using FinanceControl.Contracts.Dtos.Common;
using FinanceControl.Contracts.Filters;

namespace FinanceControl.Client.Services.Interfaces.Categories;

public interface ICategoryCliService
{
    Task<DataResultDto<CategoryDto>?> ListAsync(DataFilterDto? filter = null);

    Task<CategoryDto?> GetByIdAsync(int id);

    Task<HttpResponseMessage> CreateAsync(CategoryRegisterDto dto);

    Task<HttpResponseMessage> UpdateAsync(int id, CategoryUpdateDto dto);

    Task<HttpResponseMessage> DeleteAsync(int id);
}
