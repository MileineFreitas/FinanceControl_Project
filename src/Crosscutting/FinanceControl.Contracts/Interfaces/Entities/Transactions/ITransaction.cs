using FinanceControl.Contracts.Enumerators.Transactions;

namespace FinanceControl.Contracts.Interfaces.Entities.Transactions;

public interface ITransaction
{
    Guid TransactionId { get; set; }

    string TransactionDescription { get; set; }

    decimal TransactionValue { get; set; }

    DateTimeOffset Date { get; set; }

    TransactionTypeKind TransactionTypeKind { get; set; }

    PaymentKind? PaymentKind { get; set; }

    Guid CategoryId { get; set; }

    int AccountId { get; set; }

    int UserId { get; set; }

    TransactionStatus Status { get; set; }

    DateTimeOffset CreatedAt { get; set; }

    DateTimeOffset? UpdatedAt { get; set; }
}
