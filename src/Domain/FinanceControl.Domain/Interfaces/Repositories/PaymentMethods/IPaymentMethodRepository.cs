using FinanceControl.Contracts.Dtos.PaymentMethods;
using FinanceControl.Domain.Entities.PaymentMethods;

namespace FinanceControl.Domain.Interfaces.Repositories.PaymentMethods;

public interface IPaymentMethodRepository
{
    Task<IReadOnlyList<PaymentMethodDto>> ListAsync(bool activeOnly = true);

    Task<PaymentMethodDto?> GetByIdAsync(Guid id);

    Task<bool> NameExistsAsync(string name, Guid? excludeId = null);

    Task<PaymentMethod> AddAsync(PaymentMethod entity);

    Task<PaymentMethod?> FindTrackedAsync(Guid id);

    Task SaveChangesAsync();

    Task<bool> DeleteAsync(Guid id);

    Task<bool> IsInUseAsync(Guid id);
}
