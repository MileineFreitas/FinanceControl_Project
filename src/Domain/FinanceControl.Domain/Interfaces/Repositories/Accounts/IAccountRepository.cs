using FinanceControl.Contracts.Dtos.Accounts;
using FinanceControl.Contracts.Dtos.Common;
using FinanceControl.Contracts.Filters;
using FinanceControl.Domain.Entities.Accounts;

namespace FinanceControl.Domain.Interfaces.Repositories.Accounts;

public interface IAccountRepository
{
    Task<DataResultDto<AccountDto>> FilterAsync(DataFilterDto filter, CancellationToken cancellationToken = default);

    Task<AccountDto?> GetByIdAsync(int accountId, CancellationToken cancellationToken = default);

    Task<Account> AddAsync(Account account, CancellationToken cancellationToken = default);

    Task<Account?> FindTrackedAsync(int id, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task<bool> HasTransactionsAsync(int accountId, CancellationToken cancellationToken = default);

    Task AdjustBalanceAsync(int accountId, decimal delta, CancellationToken cancellationToken = default);
}
