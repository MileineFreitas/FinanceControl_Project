using FinanceControl.Contracts.Dtos.Accounts;
using FinanceControl.Contracts.Dtos.Common;
using FinanceControl.Contracts.Filters;
using FinanceControl.Domain.Entities.Accounts;

namespace FinanceControl.Domain.Interfaces.Repositories.Accounts;

public interface IAccountRepository
{
    Task<DataResultDto<AccountDto>> FilterAsync(DataFilterDto filter);

    Task<AccountDto?> GetByIdAsync(int accountId);

    Task<Account> AddAsync(Account account);

    Task<Account?> FindTrackedAsync(int id);

    Task SaveChangesAsync();

    Task<bool> DeleteAsync(int id);

    Task<bool> HasTransactionsAsync(int accountId);

    Task AdjustBalanceAsync(int accountId, decimal delta);
}
