using FinanceControl.Contracts.Dtos.Common;
using FinanceControl.Contracts.Dtos.Transactions;
using FinanceControl.Contracts.Filters;
using FinanceControl.Domain.Entities.Transactions;

namespace FinanceControl.Domain.Interfaces.Repositories.Transactions;

public interface ITransactionRepository
{
    Task<DataResultDto<TransactionDto>> FilterAsync(DataFilterDto filter, CancellationToken cancellationToken = default);

    Task<TransactionDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Transaction> AddAsync(Transaction transaction, CancellationToken cancellationToken = default);

    Task<Transaction?> FindTrackedAsync(Guid id, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> CategoryExistsAsync(Guid categoryId, CancellationToken cancellationToken = default);

    Task<bool> AccountExistsAsync(Guid accountId, CancellationToken cancellationToken = default);

    Task<bool> UserExistsAsync(Guid userId, CancellationToken cancellationToken = default);
}
