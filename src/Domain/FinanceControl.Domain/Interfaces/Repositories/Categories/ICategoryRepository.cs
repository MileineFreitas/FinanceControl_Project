using FinanceControl.Contracts.Dtos.Categories;
using FinanceControl.Contracts.Dtos.Common;
using FinanceControl.Contracts.Filters;
using FinanceControl.Domain.Entities.Categories;

namespace FinanceControl.Domain.Interfaces.Repositories.Categories;

public interface ICategoryRepository
{
    Task<DataResultDto<CategoryDto>> FilterAsync(DataFilterDto filter, CancellationToken cancellationToken = default);

    Task<CategoryDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Category> AddAsync(Category category, CancellationToken cancellationToken = default);

    Task<Category?> FindTrackedAsync(int id, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task<int?> GetFirstUserIdAsync(CancellationToken cancellationToken = default);
}
