using FinanceControl.Contracts.Dtos.PaymentMethods;
using FinanceControl.Domain.Entities.PaymentMethods;
using FinanceControl.Domain.Interfaces.DomService.PaymentMethods;
using FinanceControl.Domain.MapperProfiles.PaymentMethods;

namespace FinanceControl.Domain.Services.PaymentMethods;

public class PaymentMethodDomService : IPaymentMethodDomService
{
    public PaymentMethod CreateFromDto(PaymentMethodCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name) || dto.Name.Trim().Length < 2)
            throw new ArgumentException("Nome deve ter pelo menos 2 caracteres.");

        return PaymentMethodMapper.ToEntity(dto);
    }

    public void ApplyUpdate(PaymentMethod entity, PaymentMethodUpdateDto dto) =>
        PaymentMethodMapper.ApplyUpdate(entity, dto);
}
