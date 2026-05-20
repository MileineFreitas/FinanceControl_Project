using FinanceControl.Contracts.Dtos.Categories;
using FinanceControl.Contracts.Dtos.Common;
using FinanceControl.Contracts.Filters;
using FinanceControl.Domain.Entities.Categories;
using FinanceControl.Domain.Interfaces.Repositories.Categories;
using FinanceControl.Domain.MapperProfiles.Categories;
using FinanceControl.Infrastructure.Contexts;
using FinanceControl.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace FinanceControl.Infrastructure.Repositories.Categories;

public class CategoryRepository(FinanceDbContext context) : ICategoryRepository
{
    public async Task<DataResultDto<CategoryDto>> FilterAsync(DataFilterDto filter, CancellationToken cancellationToken = default)
    {
        var query = context.Categories
            .AsNoTracking()
            .OrderBy(c => c.CategoryName)
            .AsQueryable();

        if (filter.Filters != null &&
            filter.Filters.TryGetValue("search", out var search) &&
            !string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLowerInvariant();
            query = query.Where(c => c.CategoryName != null && c.CategoryName.ToLower().Contains(term));
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Page(filter.Page, filter.PageSize)
            .ToListAsync(cancellationToken);

        return new DataResultDto<CategoryDto>
        {
            Page = filter.Page,
            Total = total,
            Result = items.Select(CategoryMapper.ToDto).ToList()
        };
    }

    public async Task<CategoryDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CategoryId == id, cancellationToken);

        return entity == null ? null : CategoryMapper.ToDto(entity);
    }

    public async Task<Category> AddAsync(Category category, CancellationToken cancellationToken = default)
    {
        context.Categories.Add(category);
        await context.SaveChangesAsync(cancellationToken);
        return category;
    }

    public Task<Category?> FindTrackedAsync(int id, CancellationToken cancellationToken = default) =>
        context.Categories.FirstOrDefaultAsync(c => c.CategoryId == id, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await context.Categories.FirstOrDefaultAsync(c => c.CategoryId == id, cancellationToken);
        if (entity == null) return false;

        context.Categories.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<int?> GetFirstUserIdAsync(CancellationToken cancellationToken = default) =>
        await context.Users.OrderBy(u => u.UserId).Select(u => (int?)u.UserId).FirstOrDefaultAsync(cancellationToken);
}
