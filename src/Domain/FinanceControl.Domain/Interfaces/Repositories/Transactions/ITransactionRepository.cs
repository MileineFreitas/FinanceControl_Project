using FinanceControl.Contracts.Dtos.Common;
using FinanceControl.Contracts.Dtos.Transactions;
using FinanceControl.Contracts.Filters;
using FinanceControl.Domain.Entities.Transactions;

namespace FinanceControl.Domain.Interfaces.Repositories.Transactions;

public interface ITransactionRepository
{
    Task<DataResultDto<TransactionDto>> FilterAsync(DataFilterDto filter, CancellationToken cancellationToken = default);

    Task<TransactionDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Transaction> AddAsync(Transaction transaction, CancellationToken cancellationToken = default);

    Task<Transaction?> FindTrackedAsync(int id, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task<bool> CategoryExistsAsync(int categoryId, CancellationToken cancellationToken = default);

    Task<bool> AccountExistsAsync(int accountId, CancellationToken cancellationToken = default);

    Task<bool> UserExistsAsync(int userId, CancellationToken cancellationToken = default);
}
