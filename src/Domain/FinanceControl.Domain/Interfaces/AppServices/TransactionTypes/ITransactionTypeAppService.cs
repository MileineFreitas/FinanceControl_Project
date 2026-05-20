using FinanceControl.Contracts.Dtos.TransactionTypes;

namespace FinanceControl.Domain.Interfaces.AppServices.TransactionTypes;

public interface ITransactionTypeAppService
{
    Task<IReadOnlyList<TransactionTypeDto>> ListAsync(bool activeOnly = true, CancellationToken cancellationToken = default);

    Task<TransactionTypeDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<TransactionTypeDto> CreateAsync(TransactionTypeCreateDto dto, CancellationToken cancellationToken = default);

    Task<TransactionTypeDto?> UpdateAsync(TransactionTypeUpdateDto dto, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
