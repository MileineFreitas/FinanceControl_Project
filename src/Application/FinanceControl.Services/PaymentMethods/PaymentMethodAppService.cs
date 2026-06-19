using FinanceControl.Contracts.Dtos.PaymentMethods;
using FinanceControl.Domain.Interfaces.AppServices.PaymentMethods;
using FinanceControl.Domain.Interfaces.DomService.PaymentMethods;
using FinanceControl.Domain.Interfaces.Repositories.PaymentMethods;

namespace FinanceControl.Services.PaymentMethods;

public class PaymentMethodAppService(
    IPaymentMethodRepository repository,
    IPaymentMethodDomService domService) : IPaymentMethodAppService
{
    public Task<IReadOnlyList<PaymentMethodDto>> ListAsync(bool activeOnly = true) =>
        repository.ListAsync(activeOnly);

    public Task<PaymentMethodDto?> GetByIdAsync(Guid id) =>
        repository.GetByIdAsync(id);

    public async Task<PaymentMethodDto> CreateAsync(PaymentMethodCreateDto dto)
    {
        if (await repository.NameExistsAsync(dto.Name))
            throw new InvalidOperationException($"Já existe um meio de pagamento com o nome '{dto.Name}'.");

        var entity = domService.CreateFromDto(dto);
        await repository.AddAsync(entity);
        return (await repository.GetByIdAsync(entity.PaymentMethodId))!;
    }

    public async Task<PaymentMethodDto?> UpdateAsync(PaymentMethodUpdateDto dto)
    {
        if (await repository.NameExistsAsync(dto.Name, dto.PaymentMethodId))
            throw new InvalidOperationException($"Já existe outro meio de pagamento com o nome '{dto.Name}'.");

        var entity = await repository.FindTrackedAsync(dto.PaymentMethodId);
        if (entity == null) return null;

        domService.ApplyUpdate(entity, dto);
        await repository.SaveChangesAsync();
        return await repository.GetByIdAsync(entity.PaymentMethodId);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await repository.FindTrackedAsync(id);
        if (entity == null) return false;

        if (await repository.IsInUseAsync(id))
            throw new InvalidOperationException("Não é possível excluir: existem transações vinculadas a este meio de pagamento.");

        return await repository.DeleteAsync(id);
    }
}
