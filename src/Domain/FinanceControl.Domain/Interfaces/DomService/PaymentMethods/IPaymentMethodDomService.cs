using FinanceControl.Contracts.Dtos.PaymentMethods;
using FinanceControl.Domain.Entities.PaymentMethods;

namespace FinanceControl.Domain.Interfaces.DomService.PaymentMethods;

public interface IPaymentMethodDomService
{
    PaymentMethod CreateFromDto(PaymentMethodCreateDto dto);

    void ApplyUpdate(PaymentMethod entity, PaymentMethodUpdateDto dto);

    void EnsureCanDelete(PaymentMethod entity);
}
