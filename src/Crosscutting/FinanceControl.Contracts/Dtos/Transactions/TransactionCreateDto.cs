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

    public DateTime Date { get; set; }

    /// <summary>Receita ou despesa — informado somente no cadastro da transação.</summary>
    [Range(1, 2)]
    public TransactionTypeKind TransactionTypeKind { get; set; }

    /// <summary>Meio de pagamento (opcional), informado somente no cadastro da transação.</summary>
    public PaymentKind? PaymentKind { get; set; }

    public int CategoryId { get; set; }

    public int AccountId { get; set; }

    public int UserId { get; set; }

    public TransactionStatus Status { get; set; } = TransactionStatus.Pago;
}
