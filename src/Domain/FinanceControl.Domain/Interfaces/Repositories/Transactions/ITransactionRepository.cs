using FinanceControl.Contracts.Dtos.Common;
using FinanceControl.Contracts.Dtos.Transactions;
using FinanceControl.Contracts.Filters;
using FinanceControl.Domain.Entities.Transactions;

namespace FinanceControl.Domain.Interfaces.Repositories.Transactions;

public interface ITransactionRepository
{
    Task<DataResultDto<TransactionDto>> FilterAsync(DataFilterDto filter);

    Task<TransactionDto?> GetByIdAsync(Guid id);

    Task<Transaction> AddAsync(Transaction transaction);

    Task<Transaction?> FindTrackedAsync(Guid id);

    Task SaveChangesAsync();

    Task<bool> DeleteAsync(Guid id);

    Task<bool> CategoryExistsAsync(Guid categoryId);

    Task<bool> CategoryIsActiveAsync(Guid categoryId);

    Task<bool> AccountExistsAsync(Guid accountId);

    Task<bool> UserExistsAsync(Guid userId);

    Task<bool> PaymentMethodExistsAsync(Guid paymentMethodId, bool requireActive = true);
}
