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
            .Include(t => t.Account)
            .Include(t => t.PaymentMethod);

    public async Task<DataResultDto<TransactionDto>> FilterAsync(DataFilterDto filter)
    {
        var query = BaseQuery()
            .OrderByDescending(t => t.Date)
            .ThenByDescending(t => t.TransactionId)
            .AsQueryable();

        if (filter.Filters != null &&
            filter.Filters.TryGetValue("userId", out var userIdStr) &&
            Guid.TryParse(userIdStr, out var userId))
        {
            query = query.Where(t => t.UserId == userId);
        }

        if (filter.Filters != null &&
            filter.Filters.TryGetValue("accountId", out var accountIdStr) &&
            Guid.TryParse(accountIdStr, out var accountId))
        {
            query = query.Where(t => t.AccountId == accountId);
        }

        var total = await query.CountAsync();
        var items = await query
            .Page(filter.Page, filter.PageSize)
            .ToListAsync();

        return new DataResultDto<TransactionDto>
        {
            Page = filter.Page,
            Total = total,
            Result = items.Select(t => TransactionMapper.ToDto(t)).ToList()
        };
    }

    public async Task<TransactionDto?> GetByIdAsync(Guid id)
    {
        var entity = await BaseQuery()
            .FirstOrDefaultAsync(t => t.TransactionId == id);

        return entity == null ? null : TransactionMapper.ToDto(entity);
    }

    public async Task<Transaction> AddAsync(Transaction transaction)
    {
        context.Transactions.Add(transaction);
        await context.SaveChangesAsync();
        return transaction;
    }

    public Task<Transaction?> FindTrackedAsync(Guid id) =>
        context.Transactions.FirstOrDefaultAsync(t => t.TransactionId == id);

    public Task SaveChangesAsync() =>
        context.SaveChangesAsync();

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await context.Transactions.FirstOrDefaultAsync(t => t.TransactionId == id);
        if (entity == null) return false;

        context.Transactions.Remove(entity);
        await context.SaveChangesAsync();
        return true;
    }

    public Task<bool> CategoryExistsAsync(Guid categoryId) =>
        context.Categories.AnyAsync(c => c.CategoryId == categoryId);

    public Task<bool> AccountExistsAsync(Guid accountId) =>
        context.Accounts.AnyAsync(a => a.AccountId == accountId);

    public Task<bool> UserExistsAsync(Guid userId) =>
        context.Users.AnyAsync(u => u.UserId == userId);

    public Task<bool> PaymentMethodExistsAsync(Guid paymentMethodId) =>
        context.PaymentMethods.AnyAsync(p => p.PaymentMethodId == paymentMethodId && p.IsActive);
}
