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
    public async Task<DataResultDto<AccountDto>> FilterAsync(DataFilterDto filter)
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

        var total = await query.CountAsync();
        var items = await query
            .Page(filter.Page, filter.PageSize)
            .ToListAsync();

        return new DataResultDto<AccountDto>
        {
            Page = filter.Page,
            Total = total,
            Result = items.Select(AccountMapper.ToDto).ToList()
        };
    }

    public async Task<AccountDto?> GetByIdAsync(int accountId)
    {
        var entity = await context.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.AccountId == accountId);

        return entity == null ? null : AccountMapper.ToDto(entity);
    }

    public async Task<Account> AddAsync(Account account)
    {
        context.Accounts.Add(account);
        await context.SaveChangesAsync();
        return account;
    }

    public Task<Account?> FindTrackedAsync(int id) =>
        context.Accounts.FirstOrDefaultAsync(a => a.AccountId == id);

    public Task SaveChangesAsync() =>
        context.SaveChangesAsync();

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await context.Accounts.FirstOrDefaultAsync(a => a.AccountId == id);
        if (entity == null) return false;

        context.Accounts.Remove(entity);
        await context.SaveChangesAsync();
        return true;
    }

    public Task<bool> HasTransactionsAsync(int accountId) =>
        context.Transactions.AnyAsync(t => t.AccountId == accountId);

    public async Task AdjustBalanceAsync(int accountId, decimal delta)
    {
        var account = await context.Accounts.FindAsync([accountId]);
        if (account == null) return;

        account.CurrentBalance += delta;
        await context.SaveChangesAsync();
    }
}
