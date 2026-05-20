using FinanceControl.Contracts.Enumerators.Transactions;

namespace FinanceControl.Contracts.Interfaces.Entities.TransactionTypes;

public interface ITransactionType
{
    int TransactionTypeId { get; set; }

    string Name { get; set; }

    string Code { get; set; }

    string Icon { get; set; }

    PaymentKind? PaymentKind { get; set; }

    string? Description { get; set; }

    bool IsSystem { get; set; }

    bool IsActive { get; set; }
}
