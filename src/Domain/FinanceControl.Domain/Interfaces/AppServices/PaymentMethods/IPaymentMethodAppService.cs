using FinanceControl.Contracts.Dtos.PaymentMethods;

namespace FinanceControl.Domain.Interfaces.AppServices.PaymentMethods;

public interface IPaymentMethodAppService
{
    Task<IReadOnlyList<PaymentMethodDto>> ListAsync(bool activeOnly = true, Guid? userId = null);

    Task<PaymentMethodDto?> GetByIdAsync(Guid id);

    Task<PaymentMethodDto> CreateAsync(PaymentMethodCreateDto dto);

    Task<PaymentMethodDto?> UpdateAsync(PaymentMethodUpdateDto dto);

    Task<bool> DeleteAsync(Guid id);
}
