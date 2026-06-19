using FinanceControl.Contracts.Dtos.Transactions;
using FinanceControl.Contracts.Enumerators.Transactions;
using FinanceControl.Domain.Entities.Transactions;

namespace FinanceControl.Domain.Interfaces.DomService.Transactions;

public interface ITransactionDomService
{
    Transaction CreateFromCreateDto(TransactionCreateDto dto);

    void ApplyUpdate(Transaction entity, TransactionUpdateDto dto);

    decimal GetBalanceDelta(decimal value, TransactionTypeKind typeKind);
}
