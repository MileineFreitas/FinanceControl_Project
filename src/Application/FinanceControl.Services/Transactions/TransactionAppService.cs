using FinanceControl.Contracts.Dtos.Common;
using FinanceControl.Contracts.Dtos.Transactions;
using FinanceControl.Contracts.Enumerators.Transactions;
using FinanceControl.Contracts.Filters;
using FinanceControl.Domain.Interfaces.AppServices.Transactions;
using FinanceControl.Domain.Interfaces.DomService.Transactions;
using FinanceControl.Domain.Interfaces.Repositories.Accounts;
using FinanceControl.Domain.Interfaces.Repositories.Transactions;
using FinanceControl.Domain.MapperProfiles.Transactions;

namespace FinanceControl.Services.Transactions;

public class TransactionAppService(
    ITransactionRepository repository,
    ITransactionDomService domService,
    IAccountRepository accountRepository) : ITransactionAppService
{
    public Task<DataResultDto<TransactionDto>> FilterAsync(DataFilterDto filter, CancellationToken cancellationToken = default) =>
        repository.FilterAsync(filter, cancellationToken);

    public Task<TransactionDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        repository.GetByIdAsync(id, cancellationToken);

    public async Task<TransactionDto> CreateAsync(TransactionCreateDto dto, CancellationToken cancellationToken = default)
    {
        await ValidateReferencesAsync(dto.CategoryId, dto.AccountId, dto.UserId, cancellationToken);

        var entity = domService.CreateFromCreateDto(dto);
        await repository.AddAsync(entity, cancellationToken);

        if (entity.Status == TransactionStatus.Pago)
        {
            var delta = domService.GetBalanceDelta(entity.TransactionValue, entity.TransactionTypeKind);
            await accountRepository.AdjustBalanceAsync(entity.AccountId, delta, cancellationToken);
        }

        return await repository.GetByIdAsync(entity.TransactionId, cancellationToken)
               ?? TransactionMapper.ToDto(entity);
    }

    public async Task<bool> UpdateAsync(int id, TransactionUpdateDto dto, CancellationToken cancellationToken = default)
    {
        if (id != dto.TransactionId) return false;

        var entity = await repository.FindTrackedAsync(id, cancellationToken);
        if (entity == null) return false;

        if (!await repository.CategoryExistsAsync(dto.CategoryId, cancellationToken))
            throw new InvalidOperationException($"Não existe categoria com CategoryId={dto.CategoryId}.");
        if (!await repository.AccountExistsAsync(dto.AccountId, cancellationToken))
            throw new InvalidOperationException($"Conta AccountId={dto.AccountId} não encontrada.");

        var oldAccountId = entity.AccountId;
        var oldValue = entity.TransactionValue;
        var oldKind = entity.TransactionTypeKind;
        var oldStatus = entity.Status;

        if (oldStatus == TransactionStatus.Pago)
        {
            var revertDelta = -domService.GetBalanceDelta(oldValue, oldKind);
            await accountRepository.AdjustBalanceAsync(oldAccountId, revertDelta, cancellationToken);
        }

        domService.ApplyUpdate(entity, dto);
        await repository.SaveChangesAsync(cancellationToken);

        if (entity.Status == TransactionStatus.Pago)
        {
            var delta = domService.GetBalanceDelta(entity.TransactionValue, entity.TransactionTypeKind);
            await accountRepository.AdjustBalanceAsync(entity.AccountId, delta, cancellationToken);
        }

        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await repository.FindTrackedAsync(id, cancellationToken);
        if (entity == null) return false;

        if (entity.Status == TransactionStatus.Pago)
        {
            var revertDelta = -domService.GetBalanceDelta(entity.TransactionValue, entity.TransactionTypeKind);
            await accountRepository.AdjustBalanceAsync(entity.AccountId, revertDelta, cancellationToken);
        }

        return await repository.DeleteAsync(id, cancellationToken);
    }

    private async Task ValidateReferencesAsync(int categoryId, int accountId, int userId, CancellationToken cancellationToken)
    {
        if (!await repository.CategoryExistsAsync(categoryId, cancellationToken))
            throw new InvalidOperationException($"Não existe categoria com CategoryId={categoryId}. Cadastre categorias antes de lançar transações.");
        if (!await repository.AccountExistsAsync(accountId, cancellationToken))
            throw new InvalidOperationException($"Conta AccountId={accountId} não encontrada.");
        if (!await repository.UserExistsAsync(userId, cancellationToken))
            throw new InvalidOperationException($"Utilizador UserId={userId} não encontrado.");
    }
}
