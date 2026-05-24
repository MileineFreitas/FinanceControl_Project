using FinanceControl.Contracts.Dtos.Categories;
using FinanceControl.Contracts.Dtos.Common;
using FinanceControl.Contracts.Filters;
using FinanceControl.Domain.Entities.Categories;

namespace FinanceControl.Domain.Interfaces.Repositories.Categories;

public interface ICategoryRepository
{
    Task<DataResultDto<CategoryDto>> FilterAsync(DataFilterDto filter);

    Task<CategoryDto?> GetByIdAsync(int id);

    Task<Category> AddAsync(Category category);

    Task<Category?> FindTrackedAsync(int id);

    Task SaveChangesAsync();

    Task<bool> DeleteAsync(int id);

    Task<int?> GetFirstUserIdAsync();
}
