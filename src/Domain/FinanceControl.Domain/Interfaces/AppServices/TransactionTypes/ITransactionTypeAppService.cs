using FinanceControl.Contracts.Dtos.TransactionTypes;

namespace FinanceControl.Domain.Interfaces.AppServices.TransactionTypes;

public interface ITransactionTypeAppService
{
    Task<IReadOnlyList<TransactionTypeDto>> ListAsync(bool activeOnly = true);

    Task<TransactionTypeDto?> GetByIdAsync(Guid id);

    Task<TransactionTypeDto> CreateAsync(TransactionTypeCreateDto dto);

    Task<TransactionTypeDto?> UpdateAsync(TransactionTypeUpdateDto dto);

    Task<bool> DeleteAsync(Guid id);
}
