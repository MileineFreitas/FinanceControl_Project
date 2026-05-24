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
    public Task<DataResultDto<AccountDto>> FilterAsync(DataFilterDto filter) =>
        repository.FilterAsync(filter);

    public Task<AccountDto?> GetByIdAsync(int id) =>
        repository.GetByIdAsync(id);

    public async Task<AccountDto> CreateAsync(AccountCreateDto dto)
    {
        var entity = domService.CreateFromCreateDto(dto);
        await repository.AddAsync(entity);
        return (await repository.GetByIdAsync(entity.AccountId))!;
    }

    public async Task<AccountDto?> UpdateAsync(AccountUpdateDto dto)
    {
        var entity = await repository.FindTrackedAsync(dto.AccountId);
        if (entity == null) return null;

        domService.ApplyUpdate(entity, dto);
        await repository.SaveChangesAsync();
        return await repository.GetByIdAsync(entity.AccountId);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var hasTx = await repository.HasTransactionsAsync(id);
        domService.ValidateDelete(id, hasTx);
        return await repository.DeleteAsync(id);
    }
}
