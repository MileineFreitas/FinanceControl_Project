using FinanceControl.Contracts.Dtos.Common;
using FinanceControl.Contracts.Dtos.Transactions;
using FinanceControl.Contracts.Filters;

namespace FinanceControl.Domain.Interfaces.AppServices.Transactions;

public interface ITransactionAppService
{
    Task<DataResultDto<TransactionDto>> FilterAsync(DataFilterDto filter, CancellationToken cancellationToken = default);

    Task<TransactionDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<TransactionDto> CreateAsync(TransactionCreateDto dto, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(Guid id, TransactionUpdateDto dto, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
