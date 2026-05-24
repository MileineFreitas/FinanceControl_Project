using FinanceControl.Contracts.Dtos.TransactionTypes;
using FinanceControl.Domain.Entities.TransactionTypes;

namespace FinanceControl.Domain.Interfaces.Repositories.TransactionTypes;

public interface ITransactionTypeRepository
{
    Task<IReadOnlyList<TransactionTypeDto>> ListAsync(bool activeOnly = true);

    Task<TransactionTypeDto?> GetByIdAsync(Guid id);

    Task<bool> NameExistsAsync(string name, Guid? excludeId = null);

    Task<bool> ExistsAsync(Guid id);

    Task<TransactionTypeDefinition> AddAsync(TransactionTypeDefinition entity);

    Task<TransactionTypeDefinition?> FindTrackedAsync(Guid id);

    Task SaveChangesAsync();

    Task<bool> DeleteAsync(Guid id);

    Task<bool> IsInUseAsync(Guid id);
}
