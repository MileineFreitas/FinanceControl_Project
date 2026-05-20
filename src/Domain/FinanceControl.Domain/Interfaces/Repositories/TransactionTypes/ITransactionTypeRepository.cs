using FinanceControl.Contracts.Dtos.TransactionTypes;
using FinanceControl.Contracts.Filters;
using FinanceControl.Domain.Entities.TransactionTypes;

namespace FinanceControl.Domain.Interfaces.Repositories.TransactionTypes;

public interface ITransactionTypeRepository
{
    Task<IReadOnlyList<TransactionTypeDto>> ListAsync(bool activeOnly = true, CancellationToken cancellationToken = default);

    Task<TransactionTypeDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<bool> CodeExistsAsync(string code, int? excludeId = null, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);

    Task<TransactionTypeDefinition> AddAsync(TransactionTypeDefinition entity, CancellationToken cancellationToken = default);

    Task<TransactionTypeDefinition?> FindTrackedAsync(int id, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task<bool> IsInUseAsync(int id, CancellationToken cancellationToken = default);

    Task<int?> GetFirstUserIdAsync(CancellationToken cancellationToken = default);
}
