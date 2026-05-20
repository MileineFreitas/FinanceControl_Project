using FinanceControl.Contracts.Dtos.Accounts;
using FinanceControl.Contracts.Dtos.Common;
using FinanceControl.Contracts.Filters;

namespace FinanceControl.Domain.Interfaces.AppServices.Accounts;

public interface IAccountAppService
{
    Task<DataResultDto<AccountDto>> FilterAsync(DataFilterDto filter, CancellationToken cancellationToken = default);

    Task<AccountDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<AccountDto> CreateAsync(AccountCreateDto dto, CancellationToken cancellationToken = default);

    Task<AccountDto?> UpdateAsync(AccountUpdateDto dto, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
