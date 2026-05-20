using FinanceControl.Contracts.Dtos.Common;
using FinanceControl.Contracts.Dtos.Transactions;
using FinanceControl.Contracts.Filters;
using FinanceControl.Domain.Entities.Transactions;
using FinanceControl.Domain.Interfaces.Repositories.Transactions;
using FinanceControl.Domain.MapperProfiles.Transactions;
using FinanceControl.Infrastructure.Contexts;
using FinanceControl.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace FinanceControl.Infrastructure.Repositories.Transactions;

public class TransactionRepository(FinanceDbContext context) : ITransactionRepository
{
    private IQueryable<Transaction> BaseQuery() =>
        context.Transactions
            .AsNoTracking()
            .Include(t => t.Category)
            .Include(t => t.Account);

    public async Task<DataResultDto<TransactionDto>> FilterAsync(DataFilterDto filter, CancellationToken cancellationToken = default)
    {
        var query = BaseQuery()
            .OrderByDescending(t => t.Date)
            .ThenByDescending(t => t.TransactionId)
            .AsQueryable();

        if (filter.Filters != null &&
            filter.Filters.TryGetValue("userId", out var userIdStr) &&
            int.TryParse(userIdStr, out var userId))
        {
            query = query.Where(t => t.UserId == userId);
        }

        if (filter.Filters != null &&
            filter.Filters.TryGetValue("accountId", out var accountIdStr) &&
            int.TryParse(accountIdStr, out var accountId))
        {
            query = query.Where(t => t.AccountId == accountId);
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Page(filter.Page, filter.PageSize)
            .ToListAsync(cancellationToken);

        return new DataResultDto<TransactionDto>
        {
            Page = filter.Page,
            Total = total,
            Result = items.Select(t => TransactionMapper.ToDto(t)).ToList()
        };
    }

    public async Task<TransactionDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await BaseQuery()
            .FirstOrDefaultAsync(t => t.TransactionId == id, cancellationToken);

        return entity == null ? null : TransactionMapper.ToDto(entity);
    }

    public async Task<Transaction> AddAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        context.Transactions.Add(transaction);
        await context.SaveChangesAsync(cancellationToken);
        return transaction;
    }

    public Task<Transaction?> FindTrackedAsync(int id, CancellationToken cancellationToken = default) =>
        context.Transactions.FirstOrDefaultAsync(t => t.TransactionId == id, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await context.Transactions.FirstOrDefaultAsync(t => t.TransactionId == id, cancellationToken);
        if (entity == null) return false;

        context.Transactions.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public Task<bool> CategoryExistsAsync(int categoryId, CancellationToken cancellationToken = default) =>
        context.Categories.AnyAsync(c => c.CategoryId == categoryId, cancellationToken);

    public Task<bool> AccountExistsAsync(int accountId, CancellationToken cancellationToken = default) =>
        context.Accounts.AnyAsync(a => a.AccountId == accountId, cancellationToken);

    public Task<bool> UserExistsAsync(int userId, CancellationToken cancellationToken = default) =>
        context.Users.AnyAsync(u => u.UserId == userId, cancellationToken);

}
