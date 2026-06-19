using FinanceControl.Contracts.Dtos.PaymentMethods;
using FinanceControl.Client.Services.Interfaces.PaymentMethods;

namespace FinanceControl.Client.Services.Interfaces.PaymentMethods;

public interface IPaymentMethodCliService
{
    Task<IReadOnlyList<PaymentMethodDto>?> ListAsync(bool includeInactive = false);

    Task<PaymentMethodDto?> GetByIdAsync(Guid id);

    Task<HttpResponseMessage> CreateAsync(PaymentMethodCreateDto dto);

    Task<HttpResponseMessage> UpdateAsync(Guid id, PaymentMethodUpdateDto dto);

    Task<HttpResponseMessage> DeleteAsync(Guid id);
}
