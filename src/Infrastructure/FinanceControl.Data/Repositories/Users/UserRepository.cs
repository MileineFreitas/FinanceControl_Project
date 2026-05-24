using FinanceControl.Contracts.Dtos.Common;
using FinanceControl.Contracts.Dtos.Users;
using FinanceControl.Contracts.Filters;
using FinanceControl.Domain.Entities.Users;
using FinanceControl.Domain.Interfaces.Repositories.Users;
using FinanceControl.Domain.MapperProfiles.Users;
using FinanceControl.Infrastructure.Contexts;
using FinanceControl.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace FinanceControl.Infrastructure.Repositories.Users;

public class UserRepository(FinanceDbContext context) : IUserRepository
{
    public async Task<DataResultDto<UserDto>> FilterAsync(DataFilterDto filter)
    {
        var query = context.Users
            .AsNoTracking()
            .OrderBy(u => u.UserName)
            .AsQueryable();

        if (filter.Filters != null &&
            filter.Filters.TryGetValue("search", out var search) &&
            !string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLowerInvariant();
            query = query.Where(u =>
                (u.UserName != null && u.UserName.ToLower().Contains(term)) ||
                (u.UserEmail != null && u.UserEmail.ToLower().Contains(term)));
        }

        var total = await query.CountAsync();
        var items = await query
            .Page(filter.Page, filter.PageSize)
            .ToListAsync();

        return new DataResultDto<UserDto>
        {
            Page = filter.Page,
            Total = total,
            Result = items.Select(UserMapper.ToDto).ToList()
        };
    }

    public async Task<UserDto?> GetByIdAsync(Guid id)
    {
        var entity = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserId == id);

        return entity == null ? null : UserMapper.ToDto(entity);
    }

    public async Task<User> AddAsync(User user)
    {
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user;
    }

    public Task<User?> FindTrackedAsync(Guid id) =>
        context.Users.FirstOrDefaultAsync(u => u.UserId == id);

    public Task SaveChangesAsync() =>
        context.SaveChangesAsync();

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await context.Users.FirstOrDefaultAsync(u => u.UserId == id);
        if (entity == null) return false;

        context.Users.Remove(entity);
        await context.SaveChangesAsync();
        return true;
    }

    public Task<User?> FindByEmailAndPasswordAsync(string email, string password) =>
        context.Users.FirstOrDefaultAsync(
            u => u.UserEmail == email && u.Password == password);

    public Task<bool> EmailExistsAsync(string email) =>
        context.Users.AnyAsync(u => u.UserEmail == email);
}
