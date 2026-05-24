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

    public async Task<TransactionTypeDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await context.TransactionTypes.AsNoTracking()
            .FirstOrDefaultAsync(t => t.TransactionTypeId == id, cancellationToken);
        return entity == null ? null : TransactionTypeMapper.ToDto(entity);
    }

    public async Task<bool> NameExistsAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var normalized = name.Trim();
        var query = context.TransactionTypes.Where(t => t.Name == normalized);
        if (excludeId.HasValue)
            query = query.Where(t => t.TransactionTypeId != excludeId.Value);
        return await query.AnyAsync(cancellationToken);
    }

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.TransactionTypes.AnyAsync(t => t.TransactionTypeId == id && t.IsActive, cancellationToken);

    public async Task<TransactionTypeDefinition> AddAsync(TransactionTypeDefinition entity, CancellationToken cancellationToken = default)
    {
        context.TransactionTypes.Add(entity);
        await context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public Task<TransactionTypeDefinition?> FindTrackedAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.TransactionTypes.FirstOrDefaultAsync(t => t.TransactionTypeId == id, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await context.TransactionTypes.FirstOrDefaultAsync(t => t.TransactionTypeId == id, cancellationToken);
        if (entity == null) return false;
        context.TransactionTypes.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public Task<bool> IsInUseAsync(Guid id, CancellationToken cancellationToken = default) =>
        Task.FromResult(false);
}
