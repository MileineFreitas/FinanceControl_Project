using FinanceControl.Contracts.Dtos.TransactionTypes;
using FinanceControl.Domain.Entities.TransactionTypes;
using FinanceControl.Domain.Interfaces.Repositories.TransactionTypes;
using FinanceControl.Domain.MapperProfiles.TransactionTypes;
using FinanceControl.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace FinanceControl.Infrastructure.Repositories.TransactionTypes;

public class TransactionTypeRepository(FinanceDbContext context) : ITransactionTypeRepository
{
    public async Task<IReadOnlyList<TransactionTypeDto>> ListAsync(bool activeOnly = true)
    {
        var query = context.TransactionTypes.AsNoTracking().OrderBy(t => t.Name).AsQueryable();
        if (activeOnly)
            query = query.Where(t => t.IsActive);

        var items = await query.ToListAsync();
        return items.Select(TransactionTypeMapper.ToDto).ToList();
    }

    public async Task<TransactionTypeDto?> GetByIdAsync(Guid id)
    {
        var entity = await context.TransactionTypes.AsNoTracking()
            .FirstOrDefaultAsync(t => t.TransactionTypeId == id);
        return entity == null ? null : TransactionTypeMapper.ToDto(entity);
    }

    public async Task<bool> NameExistsAsync(string name, Guid? excludeId = null)
    {
        var normalized = name.Trim();
        var query = context.TransactionTypes.Where(t => t.Name == normalized);
        if (excludeId.HasValue)
            query = query.Where(t => t.TransactionTypeId != excludeId.Value);
        return await query.AnyAsync();
    }

    public Task<bool> ExistsAsync(Guid id) =>
        context.TransactionTypes.AnyAsync(t => t.TransactionTypeId == id && t.IsActive);

    public async Task<TransactionTypeDefinition> AddAsync(TransactionTypeDefinition entity)
    {
        context.TransactionTypes.Add(entity);
        await context.SaveChangesAsync();
        return entity;
    }

    public Task<TransactionTypeDefinition?> FindTrackedAsync(Guid id) =>
        context.TransactionTypes.FirstOrDefaultAsync(t => t.TransactionTypeId == id);

    public Task SaveChangesAsync() =>
        context.SaveChangesAsync();

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await context.TransactionTypes.FirstOrDefaultAsync(t => t.TransactionTypeId == id);
        if (entity == null) return false;
        context.TransactionTypes.Remove(entity);
        await context.SaveChangesAsync();
        return true;
    }

    public Task<bool> IsInUseAsync(Guid id) =>
        Task.FromResult(false);
}
