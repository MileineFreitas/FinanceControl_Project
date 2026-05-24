using FinanceControl.Contracts.Dtos.Accounts;
using FinanceControl.Contracts.Dtos.Common;
using FinanceControl.Contracts.Filters;

namespace FinanceControl.Domain.Interfaces.AppServices.Accounts;

public interface IAccountAppService
{
    Task<DataResultDto<AccountDto>> FilterAsync(DataFilterDto filter);

    Task<AccountDto?> GetByIdAsync(Guid id);

    Task<AccountDto> CreateAsync(AccountCreateDto dto);

    Task<AccountDto?> UpdateAsync(AccountUpdateDto dto);

    Task<bool> DeleteAsync(Guid id);
}
