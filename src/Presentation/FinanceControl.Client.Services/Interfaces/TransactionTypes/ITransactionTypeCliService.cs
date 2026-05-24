using FinanceControl.Contracts.Dtos.TransactionTypes;

namespace FinanceControl.Client.Services.Interfaces.TransactionTypes;

public interface ITransactionTypeCliService
{
    Task<IReadOnlyList<TransactionTypeDto>?> ListAsync(bool includeInactive = false);

    Task<TransactionTypeDto?> GetByIdAsync(int id);

    Task<HttpResponseMessage> CreateAsync(TransactionTypeCreateDto dto);

    Task<HttpResponseMessage> UpdateAsync(int id, TransactionTypeUpdateDto dto);

    Task<HttpResponseMessage> DeleteAsync(int id);
}
