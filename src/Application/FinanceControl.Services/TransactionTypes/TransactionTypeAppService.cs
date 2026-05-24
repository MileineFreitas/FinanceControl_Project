using FinanceControl.Contracts.Dtos.TransactionTypes;
using FinanceControl.Domain.Interfaces.AppServices.TransactionTypes;
using FinanceControl.Domain.Interfaces.DomService.TransactionTypes;
using FinanceControl.Domain.Interfaces.Repositories.TransactionTypes;

namespace FinanceControl.Services.TransactionTypes;

public class TransactionTypeAppService(
    ITransactionTypeRepository repository,
    ITransactionTypeDomService domService) : ITransactionTypeAppService
{
    public Task<IReadOnlyList<TransactionTypeDto>> ListAsync(bool activeOnly = true, CancellationToken cancellationToken = default) =>
        repository.ListAsync(activeOnly, cancellationToken);

    public Task<TransactionTypeDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        repository.GetByIdAsync(id, cancellationToken);

    public async Task<TransactionTypeDto> CreateAsync(TransactionTypeCreateDto dto, CancellationToken cancellationToken = default)
    {
        if (await repository.NameExistsAsync(dto.Name, cancellationToken: cancellationToken))
            throw new InvalidOperationException($"Já existe um tipo com o nome '{dto.Name}'.");

        var entity = domService.CreateFromDto(dto);
        await repository.AddAsync(entity, cancellationToken);
        return (await repository.GetByIdAsync(entity.TransactionTypeId, cancellationToken))!;
    }

    public async Task<TransactionTypeDto?> UpdateAsync(TransactionTypeUpdateDto dto, CancellationToken cancellationToken = default)
    {
        if (await repository.NameExistsAsync(dto.Name, dto.TransactionTypeId, cancellationToken))
            throw new InvalidOperationException($"Já existe outro tipo com o nome '{dto.Name}'.");

        var entity = await repository.FindTrackedAsync(dto.TransactionTypeId, cancellationToken);
        if (entity == null) return null;

        domService.ApplyUpdate(entity, dto);
        await repository.SaveChangesAsync(cancellationToken);
        return await repository.GetByIdAsync(entity.TransactionTypeId, cancellationToken);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await repository.FindTrackedAsync(id, cancellationToken);
        if (entity == null) return false;

        domService.EnsureCanDelete(entity);

        if (await repository.IsInUseAsync(id, cancellationToken))
            throw new InvalidOperationException("Não é possível excluir: existem transações vinculadas a este tipo.");

        return await repository.DeleteAsync(id, cancellationToken);
    }
}
