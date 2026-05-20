using FinanceControl.Contracts.Dtos.Accounts;
using FinanceControl.Contracts.Dtos.Common;
using FinanceControl.Contracts.Filters;

namespace FinanceControl.Client.Services.Interfaces.Accounts;

public interface IAccountCliService
{
    Task<IReadOnlyList<AccountDto>?> ListAsync(int? userId = null, CancellationToken cancellationToken = default);

    Task<AccountDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<HttpResponseMessage> CreateAsync(AccountCreateDto dto, CancellationToken cancellationToken = default);

    Task<HttpResponseMessage> UpdateAsync(int id, AccountUpdateDto dto, CancellationToken cancellationToken = default);

    Task<HttpResponseMessage> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
