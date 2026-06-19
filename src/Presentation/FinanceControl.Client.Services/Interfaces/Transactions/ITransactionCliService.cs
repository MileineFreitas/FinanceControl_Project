using FinanceControl.Contracts.Dtos.Common;
using FinanceControl.Contracts.Dtos.Transactions;
using FinanceControl.Contracts.Filters;

namespace FinanceControl.Client.Services.Interfaces.Transactions;

public interface ITransactionCliService
{
    Task<DataResultDto<TransactionDto>?> ListAsync(DataFilterDto? filter = null);

    Task<TransactionDto?> GetByIdAsync(Guid id);

    Task<HttpResponseMessage> CreateAsync(TransactionCreateDto dto);

    Task<HttpResponseMessage> UpdateAsync(Guid id, TransactionUpdateDto dto);

    Task<HttpResponseMessage> DeleteAsync(Guid id);
}
