using FinanceControl.Contracts.Enumerators.Transactions;
using FinanceControl.Contracts.Interfaces.Entities.TransactionTypes;

namespace FinanceControl.Contracts.Dtos.TransactionTypes;

public class TransactionTypeDto : ITransactionType
{
    public int TransactionTypeId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public string Icon { get; set; } = "💳";

    public PaymentKind? PaymentKind { get; set; }

    public string? Description { get; set; }

    public bool IsSystem { get; set; }

    public bool IsActive { get; set; }

    public string PaymentKindLabel => PaymentKind switch
    {
        Enumerators.Transactions.PaymentKind.Debit => "Débito",
        Enumerators.Transactions.PaymentKind.Credit => "Crédito",
        Enumerators.Transactions.PaymentKind.Cash => "Dinheiro",
        _ => "Personalizado"
    };
}
