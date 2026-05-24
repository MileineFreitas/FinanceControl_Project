using System.ComponentModel.DataAnnotations;
using FinanceControl.Contracts.Enumerators.Transactions;

namespace FinanceControl.Contracts.Dtos.Transactions;

public class TransactionCreateDto
{
    [Required]
    [StringLength(250)]
    public string TransactionDescription { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public decimal TransactionValue { get; set; }

    public DateTimeOffset Date { get; set; }

    [Range(1, 2)]
    public TransactionTypeKind TransactionTypeKind { get; set; }

    public Guid PaymentMethodId { get; set; }

    public Guid CategoryId { get; set; }

    public Guid AccountId { get; set; }

    public Guid UserId { get; set; }
}
