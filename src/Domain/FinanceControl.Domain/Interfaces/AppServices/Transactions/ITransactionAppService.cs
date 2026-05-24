using FinanceControl.Contracts.Dtos.Common;
using FinanceControl.Contracts.Dtos.Transactions;
using FinanceControl.Contracts.Filters;

namespace FinanceControl.Domain.Interfaces.AppServices.Transactions;

public interface ITransactionAppService
{
    Task<DataResultDto<TransactionDto>> FilterAsync(DataFilterDto filter);

    Task<TransactionDto?> GetByIdAsync(Guid id);

    Task<TransactionDto> CreateAsync(TransactionCreateDto dto);

    Task<bool> UpdateAsync(Guid id, TransactionUpdateDto dto);

    Task<bool> DeleteAsync(Guid id);
}
