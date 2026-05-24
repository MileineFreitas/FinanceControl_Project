using FinanceControl.Contracts.Dtos.TransactionTypes;
using FinanceControl.Domain.Entities.TransactionTypes;

namespace FinanceControl.Domain.Interfaces.Repositories.TransactionTypes;

public interface ITransactionTypeRepository
{
    Task<IReadOnlyList<TransactionTypeDto>> ListAsync(bool activeOnly = true, CancellationToken cancellationToken = default);

    Task<TransactionTypeDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> NameExistsAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<TransactionTypeDefinition> AddAsync(TransactionTypeDefinition entity, CancellationToken cancellationToken = default);

    Task<TransactionTypeDefinition?> FindTrackedAsync(Guid id, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> IsInUseAsync(Guid id, CancellationToken cancellationToken = default);
}
