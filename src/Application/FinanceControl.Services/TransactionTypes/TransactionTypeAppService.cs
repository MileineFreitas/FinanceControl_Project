using FinanceControl.Contracts.Dtos.TransactionTypes;
using FinanceControl.Domain.Interfaces.AppServices.TransactionTypes;
using FinanceControl.Domain.Interfaces.DomService.TransactionTypes;
using FinanceControl.Domain.Interfaces.Repositories.TransactionTypes;

namespace FinanceControl.Services.TransactionTypes;

public class TransactionTypeAppService(
    ITransactionTypeRepository repository,
    ITransactionTypeDomService domService) : ITransactionTypeAppService
{
    public Task<IReadOnlyList<TransactionTypeDto>> ListAsync(bool activeOnly = true) =>
        repository.ListAsync(activeOnly);

    public Task<TransactionTypeDto?> GetByIdAsync(Guid id) =>
        repository.GetByIdAsync(id);

    public async Task<TransactionTypeDto> CreateAsync(TransactionTypeCreateDto dto)
    {
        if (await repository.NameExistsAsync(dto.Name))
            throw new InvalidOperationException($"Já existe um tipo com o nome '{dto.Name}'.");

        var entity = domService.CreateFromDto(dto);
        await repository.AddAsync(entity);
        return (await repository.GetByIdAsync(entity.TransactionTypeId))!;
    }

    public async Task<TransactionTypeDto?> UpdateAsync(TransactionTypeUpdateDto dto)
    {
        if (await repository.NameExistsAsync(dto.Name, dto.TransactionTypeId))
            throw new InvalidOperationException($"Já existe outro tipo com o nome '{dto.Name}'.");

        var entity = await repository.FindTrackedAsync(dto.TransactionTypeId);
        if (entity == null) return null;

        domService.ApplyUpdate(entity, dto);
        await repository.SaveChangesAsync();
        return await repository.GetByIdAsync(entity.TransactionTypeId);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await repository.FindTrackedAsync(id);
        if (entity == null) return false;

        domService.EnsureCanDelete(entity);

        if (await repository.IsInUseAsync(id))
            throw new InvalidOperationException("Não é possível excluir: existem transações vinculadas a este tipo.");

        return await repository.DeleteAsync(id);
    }
}
