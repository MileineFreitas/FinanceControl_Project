using FinanceControl.Contracts.Dtos.Common;
using FinanceControl.Contracts.Dtos.Transactions;
using FinanceControl.Contracts.Filters;

namespace FinanceControl.Domain.Interfaces.AppServices.Transactions;

public interface ITransactionAppService
{
    Task<DataResultDto<TransactionDto>> FilterAsync(DataFilterDto filter, CancellationToken cancellationToken = default);

    Task<TransactionDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<TransactionDto> CreateAsync(TransactionCreateDto dto, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(int id, TransactionUpdateDto dto, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
