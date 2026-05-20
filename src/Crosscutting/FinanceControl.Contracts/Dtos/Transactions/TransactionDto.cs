using FinanceControl.Contracts.Enumerators.Transactions;

namespace FinanceControl.Contracts.Dtos.Transactions;

public class TransactionDto
{
    public int TransactionId { get; set; }

    public string TransactionDescription { get; set; } = string.Empty;

    public decimal TransactionValue { get; set; }

    public DateTime Date { get; set; }

    public TransactionTypeKind TransactionTypeKind { get; set; }

    public PaymentKind? PaymentKind { get; set; }

    public int CategoryId { get; set; }

    public string? CategoryName { get; set; }

    public int AccountId { get; set; }

    public string? AccountName { get; set; }

    public int UserId { get; set; }

    public TransactionStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
