using FinanceControl.Contracts.Dtos.PaymentMethods;
using FinanceControl.Domain.Entities.PaymentMethods;
using FinanceControl.Domain.Interfaces.Repositories.PaymentMethods;
using FinanceControl.Domain.MapperProfiles.PaymentMethods;
using FinanceControl.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace FinanceControl.Infrastructure.Repositories.PaymentMethods;

public class PaymentMethodRepository(FinanceDbContext context) : IPaymentMethodRepository
{
    public async Task<IReadOnlyList<PaymentMethodDto>> ListAsync(bool activeOnly = true)
    {
        var query = context.PaymentMethods.AsNoTracking().OrderBy(p => p.Name).AsQueryable();
        if (activeOnly)
            query = query.Where(p => p.IsActive);

        var items = await query.ToListAsync();
        return items.Select(PaymentMethodMapper.ToDto).ToList();
    }

    public async Task<PaymentMethodDto?> GetByIdAsync(Guid id)
    {
        var entity = await context.PaymentMethods.AsNoTracking()
            .FirstOrDefaultAsync(p => p.PaymentMethodId == id);
        return entity == null ? null : PaymentMethodMapper.ToDto(entity);
    }

    public async Task<bool> NameExistsAsync(string name, Guid? excludeId = null)
    {
        var normalized = name.Trim();
        var query = context.PaymentMethods.Where(p => p.Name == normalized);
        if (excludeId.HasValue)
            query = query.Where(p => p.PaymentMethodId != excludeId.Value);
        return await query.AnyAsync();
    }

    public async Task<PaymentMethod> AddAsync(PaymentMethod entity)
    {
        context.PaymentMethods.Add(entity);
        await context.SaveChangesAsync();
        return entity;
    }

    public Task<PaymentMethod?> FindTrackedAsync(Guid id) =>
        context.PaymentMethods.FirstOrDefaultAsync(p => p.PaymentMethodId == id);

    public Task SaveChangesAsync() =>
        context.SaveChangesAsync();

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await context.PaymentMethods.FirstOrDefaultAsync(p => p.PaymentMethodId == id);
        if (entity == null) return false;
        context.PaymentMethods.Remove(entity);
        await context.SaveChangesAsync();
        return true;
    }

    public Task<bool> IsInUseAsync(Guid id) =>
        context.Transactions.AnyAsync(t => t.PaymentMethodId == id);
}
