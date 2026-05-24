using FinanceControl.Contracts.Dtos.Common;
using FinanceControl.Contracts.Dtos.Transactions;
using FinanceControl.Contracts.Filters;

namespace FinanceControl.Client.Services.Interfaces.Transactions;

public interface ITransactionCliService
{
    Task<DataResultDto<TransactionDto>?> ListAsync(DataFilterDto? filter = null);

    Task<TransactionDto?> GetByIdAsync(int id);

    Task<HttpResponseMessage> CreateAsync(TransactionCreateDto dto);

    Task<HttpResponseMessage> UpdateAsync(int id, TransactionUpdateDto dto);

    Task<HttpResponseMessage> DeleteAsync(int id);
}
