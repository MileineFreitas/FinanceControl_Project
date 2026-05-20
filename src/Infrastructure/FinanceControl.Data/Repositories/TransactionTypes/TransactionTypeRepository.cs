using FinanceControl.Contracts.Dtos.TransactionTypes;
using FinanceControl.Domain.Entities.TransactionTypes;
using FinanceControl.Domain.Interfaces.Repositories.TransactionTypes;
using FinanceControl.Domain.MapperProfiles.TransactionTypes;
using FinanceControl.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace FinanceControl.Infrastructure.Repositories.TransactionTypes;

public class TransactionTypeRepository(FinanceDbContext context) : ITransactionTypeRepository
{
    public async Task<IReadOnlyList<TransactionTypeDto>> ListAsync(bool activeOnly = true, CancellationToken cancellationToken = default)
    {
        var query = context.TransactionTypes.AsNoTracking().OrderBy(t => t.Name).AsQueryable();
        if (activeOnly)
            query = query.Where(t => t.IsActive);

        var items = await query.ToListAsync(cancellationToken);
        return items.Select(TransactionTypeMapper.ToDto).ToList();
    }

    public async Task<TransactionTypeDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await context.TransactionTypes.AsNoTracking()
            .FirstOrDefaultAsync(t => t.TransactionTypeId == id, cancellationToken);
        return entity == null ? null : TransactionTypeMapper.ToDto(entity);
    }

    public async Task<bool> CodeExistsAsync(string code, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var normalized = code.Trim().ToUpperInvariant();
        var query = context.TransactionTypes.Where(t => t.Code == normalized);
        if (excludeId.HasValue)
            query = query.Where(t => t.TransactionTypeId != excludeId.Value);
        return await query.AnyAsync(cancellationToken);
    }

    public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default) =>
        context.TransactionTypes.AnyAsync(t => t.TransactionTypeId == id && t.IsActive, cancellationToken);

    public async Task<TransactionTypeDefinition> AddAsync(TransactionTypeDefinition entity, CancellationToken cancellationToken = default)
    {
        context.TransactionTypes.Add(entity);
        await context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public Task<TransactionTypeDefinition?> FindTrackedAsync(int id, CancellationToken cancellationToken = default) =>
        context.TransactionTypes.FirstOrDefaultAsync(t => t.TransactionTypeId == id, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await context.TransactionTypes.FirstOrDefaultAsync(t => t.TransactionTypeId == id, cancellationToken);
        if (entity == null) return false;
        context.TransactionTypes.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public Task<bool> IsInUseAsync(int id, CancellationToken cancellationToken = default) =>
        Task.FromResult(false);

    public async Task<int?> GetFirstUserIdAsync(CancellationToken cancellationToken = default) =>
        await context.Users.OrderBy(u => u.UserId).Select(u => (int?)u.UserId).FirstOrDefaultAsync(cancellationToken);
}
