using FinanceControl.Contracts.Dtos.Accounts;
using FinanceControl.Contracts.Dtos.Common;
using FinanceControl.Contracts.Filters;
using FinanceControl.Domain.Entities.Accounts;
using FinanceControl.Domain.Interfaces.Repositories.Accounts;

namespace FinanceControl.Domain.Interfaces.Repositories.Accounts;

public interface IAccountRepository
{
    Task<DataResultDto<AccountDto>> FilterAsync(DataFilterDto filter);

    Task<AccountDto?> GetByIdAsync(Guid accountId);

    Task<Account> AddAsync(Account account);

    Task<Account?> FindTrackedAsync(Guid id);

    Task SaveChangesAsync();

    Task<bool> DeleteAsync(Guid id);

    Task<bool> HasTransactionsAsync(Guid accountId);

    Task AdjustBalanceAsync(Guid accountId, decimal delta);

    Task<Account?> GetFirstByUserIdAsync(Guid userId);
}
