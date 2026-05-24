using FinanceControl.Contracts.Dtos.Accounts;
using FinanceControl.Contracts.Dtos.Common;
using FinanceControl.Contracts.Filters;

namespace FinanceControl.Client.Services.Interfaces.Accounts;

public interface IAccountCliService
{
    Task<IReadOnlyList<AccountDto>?> ListAsync(Guid? userId = null);

    Task<AccountDto?> GetByIdAsync(Guid id);

    Task<HttpResponseMessage> CreateAsync(AccountCreateDto dto);

    Task<HttpResponseMessage> UpdateAsync(Guid id, AccountUpdateDto dto);

    Task<HttpResponseMessage> DeleteAsync(Guid id);
}
