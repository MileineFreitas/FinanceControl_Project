using FinanceControl.Contracts.Dtos.TransactionTypes;

namespace FinanceControl.Client.Services.Interfaces.TransactionTypes;

public interface ITransactionTypeCliService
{
    Task<IReadOnlyList<TransactionTypeDto>?> ListAsync(bool includeInactive = false, CancellationToken cancellationToken = default);

    Task<TransactionTypeDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<HttpResponseMessage> CreateAsync(TransactionTypeCreateDto dto, CancellationToken cancellationToken = default);

    Task<HttpResponseMessage> UpdateAsync(int id, TransactionTypeUpdateDto dto, CancellationToken cancellationToken = default);

    Task<HttpResponseMessage> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
