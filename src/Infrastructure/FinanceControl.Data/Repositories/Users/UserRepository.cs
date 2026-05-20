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
    public async Task<DataResultDto<UserDto>> FilterAsync(DataFilterDto filter, CancellationToken cancellationToken = default)
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

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Page(filter.Page, filter.PageSize)
            .ToListAsync(cancellationToken);

        return new DataResultDto<UserDto>
        {
            Page = filter.Page,
            Total = total,
            Result = items.Select(UserMapper.ToDto).ToList()
        };
    }

    public async Task<UserDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserId == id, cancellationToken);

        return entity == null ? null : UserMapper.ToDto(entity);
    }

    public async Task<User> AddAsync(User user, CancellationToken cancellationToken = default)
    {
        context.Users.Add(user);
        await context.SaveChangesAsync(cancellationToken);
        return user;
    }

    public Task<User?> FindTrackedAsync(int id, CancellationToken cancellationToken = default) =>
        context.Users.FirstOrDefaultAsync(u => u.UserId == id, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await context.Users.FirstOrDefaultAsync(u => u.UserId == id, cancellationToken);
        if (entity == null) return false;

        context.Users.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public Task<User?> FindByEmailAndPasswordAsync(string email, string password, CancellationToken cancellationToken = default) =>
        context.Users.FirstOrDefaultAsync(
            u => u.UserEmail == email && u.Password == password,
            cancellationToken);

    public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default) =>
        context.Users.AnyAsync(u => u.UserEmail == email, cancellationToken);
}
