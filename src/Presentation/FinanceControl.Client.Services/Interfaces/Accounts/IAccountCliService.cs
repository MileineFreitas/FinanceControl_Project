using FinanceControl.Contracts.Dtos.Accounts;
using FinanceControl.Contracts.Dtos.Common;
using FinanceControl.Contracts.Filters;

namespace FinanceControl.Client.Services.Interfaces.Accounts;

public interface IAccountCliService
{
    Task<IReadOnlyList<AccountDto>?> ListAsync(int? userId = null);

    Task<AccountDto?> GetByIdAsync(int id);

    Task<HttpResponseMessage> CreateAsync(AccountCreateDto dto);

    Task<HttpResponseMessage> UpdateAsync(int id, AccountUpdateDto dto);

    Task<HttpResponseMessage> DeleteAsync(int id);
}
