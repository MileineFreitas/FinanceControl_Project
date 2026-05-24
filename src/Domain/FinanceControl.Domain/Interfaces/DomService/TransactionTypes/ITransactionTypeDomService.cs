using FinanceControl.Contracts.Dtos.TransactionTypes;
using FinanceControl.Domain.Entities.TransactionTypes;

namespace FinanceControl.Domain.Interfaces.DomService.TransactionTypes;

public interface ITransactionTypeDomService
{
    TransactionTypeDefinition CreateFromDto(TransactionTypeCreateDto dto);

    void ApplyUpdate(TransactionTypeDefinition entity, TransactionTypeUpdateDto dto);

    void EnsureCanDelete(TransactionTypeDefinition entity);
}
