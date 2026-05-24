using FinanceControl.Contracts.Enumerators.Transactions;

namespace FinanceControl.Contracts.Dtos.Transactions;

public class TransactionDto
{
    public Guid TransactionId { get; set; }

    public string TransactionDescription { get; set; } = string.Empty;

    public decimal TransactionValue { get; set; }

    public DateTimeOffset Date { get; set; }

    public TransactionTypeKind TransactionTypeKind { get; set; }

    public PaymentKind PaymentKind { get; set; }

    public Guid CategoryId { get; set; }

    public string? CategoryName { get; set; }

    public Guid AccountId { get; set; }

    public string? AccountName { get; set; }

    public Guid UserId { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }
}
