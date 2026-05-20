using FinanceControl.Contracts.Dtos.Accounts;
using FinanceControl.Contracts.Dtos.Common;
using FinanceControl.Contracts.Filters;
using FinanceControl.Domain.Interfaces.AppServices.Accounts;
using FinanceControl.Domain.Interfaces.DomService.Accounts;
using FinanceControl.Domain.Interfaces.Repositories.Accounts;

namespace FinanceControl.Services.Accounts;

public class AccountAppService(
    IAccountRepository repository,
    IAccountDomService domService) : IAccountAppService
{
    public Task<DataResultDto<AccountDto>> FilterAsync(DataFilterDto filter, CancellationToken cancellationToken = default) =>
        repository.FilterAsync(filter, cancellationToken);

    public Task<AccountDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        repository.GetByIdAsync(id, cancellationToken);

    public async Task<AccountDto> CreateAsync(AccountCreateDto dto, CancellationToken cancellationToken = default)
    {
        var entity = domService.CreateFromCreateDto(dto);
        await repository.AddAsync(entity, cancellationToken);
        return (await repository.GetByIdAsync(entity.AccountId, cancellationToken))!;
    }

    public async Task<AccountDto?> UpdateAsync(AccountUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await repository.FindTrackedAsync(dto.AccountId, cancellationToken);
        if (entity == null) return null;

        domService.ApplyUpdate(entity, dto);
        await repository.SaveChangesAsync(cancellationToken);
        return await repository.GetByIdAsync(entity.AccountId, cancellationToken);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var hasTx = await repository.HasTransactionsAsync(id, cancellationToken);
        domService.ValidateDelete(id, hasTx);
        return await repository.DeleteAsync(id, cancellationToken);
    }
}
