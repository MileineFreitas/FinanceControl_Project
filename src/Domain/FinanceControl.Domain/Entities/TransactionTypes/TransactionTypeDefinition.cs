using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;using FinanceControl.Contracts.Interfaces.Entities.TransactionTypes;
namespace FinanceControl.Domain.Entities.TransactionTypes;
[Table("TransactionTypes")]
public class TransactionTypeDefinition : ITransactionType
{
    [Key]
    public Guid TransactionTypeId { get; set; } = Guid.NewGuid();
    [Required]
    [StringLength(40)]
    public string Name { get; set; } = string.Empty;
    [Required]
    [StringLength(16)]
    public string Icon { get; set; } = "💳";
    [StringLength(200)]
    public string? Description { get; set; }

    public bool IsSystem { get; set; }
    public bool IsActive { get; set; } = true;
}