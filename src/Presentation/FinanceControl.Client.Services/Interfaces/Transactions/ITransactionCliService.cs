using FinanceControl.Contracts.Dtos.Common;
using FinanceControl.Contracts.Dtos.Transactions;
using FinanceControl.Contracts.Filters;

namespace FinanceControl.Client.Services.Interfaces.Transactions;

public interface ITransactionCliService
{
    Task<DataResultDto<TransactionDto>?> ListAsync(DataFilterDto? filter = null, CancellationToken cancellationToken = default);

    Task<TransactionDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<HttpResponseMessage> CreateAsync(TransactionCreateDto dto, CancellationToken cancellationToken = default);

    Task<HttpResponseMessage> UpdateAsync(int id, TransactionUpdateDto dto, CancellationToken cancellationToken = default);

    Task<HttpResponseMessage> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
