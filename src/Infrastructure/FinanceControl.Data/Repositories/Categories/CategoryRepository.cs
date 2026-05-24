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
    public async Task<DataResultDto<CategoryDto>> FilterAsync(DataFilterDto filter)
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

        var total = await query.CountAsync();
        var items = await query
            .Page(filter.Page, filter.PageSize)
            .ToListAsync();

        return new DataResultDto<CategoryDto>
        {
            Page = filter.Page,
            Total = total,
            Result = items.Select(CategoryMapper.ToDto).ToList()
        };
    }

    public async Task<CategoryDto?> GetByIdAsync(int id)
    {
        var entity = await context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CategoryId == id);

        return entity == null ? null : CategoryMapper.ToDto(entity);
    }

    public async Task<Category> AddAsync(Category category)
    {
        context.Categories.Add(category);
        await context.SaveChangesAsync();
        return category;
    }

    public Task<Category?> FindTrackedAsync(int id) =>
        context.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);

    public Task SaveChangesAsync() =>
        context.SaveChangesAsync();

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await context.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);
        if (entity == null) return false;

        context.Categories.Remove(entity);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<int?> GetFirstUserIdAsync() =>
        await context.Users.OrderBy(u => u.UserId).Select(u => (int?)u.UserId).FirstOrDefaultAsync();
}
