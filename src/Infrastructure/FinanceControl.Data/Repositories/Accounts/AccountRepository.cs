using FinanceControl.Contracts.Dtos.Accounts;
using FinanceControl.Contracts.Dtos.Common;
using FinanceControl.Contracts.Filters;
using FinanceControl.Domain.Entities.Accounts;
using FinanceControl.Domain.Interfaces.Repositories.Accounts;
using FinanceControl.Domain.MapperProfiles.Accounts;
using FinanceControl.Infrastructure.Contexts;
using FinanceControl.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace FinanceControl.Infrastructure.Repositories.Accounts;

public class AccountRepository(FinanceDbContext context) : IAccountRepository
{
    public async Task<DataResultDto<AccountDto>> FilterAsync(DataFilterDto filter, CancellationToken cancellationToken = default)
    {
        var query = context.Accounts
            .AsNoTracking()
            .OrderBy(a => a.Name)
            .AsQueryable();

        if (filter.Filters != null &&
            filter.Filters.TryGetValue("userId", out var userIdStr) &&
            int.TryParse(userIdStr, out var userId))
        {
            query = query.Where(a => a.UserId == userId || a.UserId == null);
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Page(filter.Page, filter.PageSize)
            .ToListAsync(cancellationToken);

        return new DataResultDto<AccountDto>
        {
            Page = filter.Page,
            Total = total,
            Result = items.Select(AccountMapper.ToDto).ToList()
        };
    }

    public async Task<AccountDto?> GetByIdAsync(int accountId, CancellationToken cancellationToken = default)
    {
        var entity = await context.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.AccountId == accountId, cancellationToken);

        return entity == null ? null : AccountMapper.ToDto(entity);
    }

    public async Task<Account> AddAsync(Account account, CancellationToken cancellationToken = default)
    {
        context.Accounts.Add(account);
        await context.SaveChangesAsync(cancellationToken);
        return account;
    }

    public Task<Account?> FindTrackedAsync(int id, CancellationToken cancellationToken = default) =>
        context.Accounts.FirstOrDefaultAsync(a => a.AccountId == id, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await context.Accounts.FirstOrDefaultAsync(a => a.AccountId == id, cancellationToken);
        if (entity == null) return false;

        context.Accounts.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public Task<bool> HasTransactionsAsync(int accountId, CancellationToken cancellationToken = default) =>
        context.Transactions.AnyAsync(t => t.AccountId == accountId, cancellationToken);

    public async Task AdjustBalanceAsync(int accountId, decimal delta, CancellationToken cancellationToken = default)
    {
        var account = await context.Accounts.FindAsync([accountId], cancellationToken);
        if (account == null) return;

        account.CurrentBalance += delta;
        await context.SaveChangesAsync(cancellationToken);
    }
}
