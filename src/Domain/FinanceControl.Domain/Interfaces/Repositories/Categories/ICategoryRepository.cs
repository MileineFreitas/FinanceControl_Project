using FinanceControl.Contracts.Dtos.Categories;
using FinanceControl.Contracts.Dtos.Common;
using FinanceControl.Contracts.Filters;
using FinanceControl.Domain.Entities.Categories;
using FinanceControl.Domain.Interfaces.Repositories.Categories;

namespace FinanceControl.Domain.Interfaces.Repositories.Categories;

public interface ICategoryRepository
{
    Task<DataResultDto<CategoryDto>> FilterAsync(DataFilterDto filter);

    Task<CategoryDto?> GetByIdAsync(Guid id);

    Task<Category> AddAsync(Category category);

    Task<Category?> FindTrackedAsync(Guid id);

    Task SaveChangesAsync();

    Task<bool> DeleteAsync(Guid id);

    Task<Guid?> GetFirstUserIdAsync();
}
