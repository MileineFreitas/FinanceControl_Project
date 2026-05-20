using System.ComponentModel.DataAnnotations;
using FinanceControl.Contracts.Enumerators.Transactions;

namespace FinanceControl.Contracts.Dtos.TransactionTypes;

public class TransactionTypeCreateDto
{
    [Required]
    [StringLength(40)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string Code { get; set; } = string.Empty;

    [StringLength(16)]
    public string Icon { get; set; } = "💳";

    public PaymentKind? PaymentKind { get; set; }

    [StringLength(200)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;
}
