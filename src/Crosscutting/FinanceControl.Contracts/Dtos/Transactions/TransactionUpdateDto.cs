using System.ComponentModel.DataAnnotations;
using FinanceControl.Contracts.Enumerators.Transactions;

namespace FinanceControl.Contracts.Dtos.Transactions;

public class TransactionUpdateDto
{
    public int TransactionId { get; set; }

    [Required]
    [StringLength(250)]
    public string TransactionDescription { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public decimal TransactionValue { get; set; }

    public DateTime Date { get; set; }

    [Range(1, 2)]
    public TransactionTypeKind TransactionTypeKind { get; set; }

    public PaymentKind? PaymentKind { get; set; }

    public int CategoryId { get; set; }

    public int AccountId { get; set; }

    public TransactionStatus Status { get; set; }
}
