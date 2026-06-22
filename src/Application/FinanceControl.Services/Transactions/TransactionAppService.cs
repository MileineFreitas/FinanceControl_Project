using FinanceControl.Contracts.Dtos.Common;
using FinanceControl.Contracts.Dtos.Transactions;
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
    public Task<DataResultDto<TransactionDto>> FilterAsync(DataFilterDto filter) =>
        repository.FilterAsync(filter);

    public Task<TransactionDto?> GetByIdAsync(Guid id) =>
        repository.GetByIdAsync(id);

    public async Task<TransactionDto> CreateAsync(TransactionCreateDto dto)
    {
        await ValidateReferencesAsync(dto.CategoryId, dto.UserId, dto.PaymentMethodId);

        var entity = domService.CreateFromCreateDto(dto);
        await repository.AddAsync(entity);

        if (entity.AccountId is Guid accountId)
        {
            var delta = domService.GetBalanceDelta(entity.TransactionValue, entity.TransactionTypeKind);
            await accountRepository.AdjustBalanceAsync(accountId, delta);
        }

        return await repository.GetByIdAsync(entity.TransactionId)
               ?? TransactionMapper.ToDto(entity);
    }

    public async Task<bool> UpdateAsync(Guid id, TransactionUpdateDto dto)
    {
        if (id != dto.TransactionId) return false;

        var entity = await repository.FindTrackedAsync(id);
        if (entity == null) return false;

        if (!await repository.CategoryExistsAsync(dto.CategoryId))
            throw new InvalidOperationException($"Não existe categoria com CategoryId={dto.CategoryId}.");
        if (!await repository.PaymentMethodExistsAsync(dto.PaymentMethodId))
            throw new InvalidOperationException($"Meio de pagamento PaymentMethodId={dto.PaymentMethodId} não encontrado ou inativo.");

        var oldAccountId = entity.AccountId;
        var oldValue = entity.TransactionValue;
        var oldKind = entity.TransactionTypeKind;

        if (oldAccountId is Guid oldAccount)
        {
            var revertDelta = -domService.GetBalanceDelta(oldValue, oldKind);
            await accountRepository.AdjustBalanceAsync(oldAccount, revertDelta);
        }

        domService.ApplyUpdate(entity, dto);
        await repository.SaveChangesAsync();

        if (entity.AccountId is Guid newAccount)
        {
            var delta = domService.GetBalanceDelta(entity.TransactionValue, entity.TransactionTypeKind);
            await accountRepository.AdjustBalanceAsync(newAccount, delta);
        }

        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await repository.FindTrackedAsync(id);
        if (entity == null) return false;

        if (entity.AccountId is Guid accountId)
        {
            var revertDelta = -domService.GetBalanceDelta(entity.TransactionValue, entity.TransactionTypeKind);
            await accountRepository.AdjustBalanceAsync(accountId, revertDelta);
        }

        return await repository.DeleteAsync(id);
    }

    private async Task ValidateReferencesAsync(Guid categoryId, Guid userId, Guid paymentMethodId)
    {
        if (!await repository.CategoryExistsAsync(categoryId))
            throw new InvalidOperationException($"Não existe categoria com CategoryId={categoryId}. Cadastre categorias antes de lançar transações.");
        if (!await repository.UserExistsAsync(userId))
            throw new InvalidOperationException($"Utilizador UserId={userId} não encontrado.");
        if (!await repository.PaymentMethodExistsAsync(paymentMethodId))
            throw new InvalidOperationException($"Meio de pagamento PaymentMethodId={paymentMethodId} não encontrado ou inativo.");
    }
}
