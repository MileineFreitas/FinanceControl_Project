using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FinanceControl.Contracts.Enumerators.Transactions;
using FinanceControl.Contracts.Interfaces.Entities.TransactionTypes;
using FinanceControl.Domain.Entities.Users;

namespace FinanceControl.Domain.Entities.TransactionTypes;

[Table("TransactionTypes")]
public class TransactionTypeDefinition : ITransactionType
{
    [Key]
    public int TransactionTypeId { get; set; }

    [Required]
    [StringLength(40)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string Code { get; set; } = string.Empty;

    public string Icon { get; set; } = "💳";

    public PaymentKind? PaymentKind { get; set; }

    [StringLength(200)]
    public string? Description { get; set; }

    public bool IsSystem { get; set; }

    public bool IsActive { get; set; } = true;

    public int? UserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }
}
