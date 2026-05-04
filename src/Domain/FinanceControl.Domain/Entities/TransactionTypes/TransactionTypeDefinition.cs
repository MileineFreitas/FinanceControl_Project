using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceControl.Domain.Entities.TransactionTypes;

/// <summary>
/// Tipos fixos no banco (RECEITA / DESPESA), populados por seed.
/// </summary>
[Table("TransactionTypes")]
public class TransactionTypeDefinition
{
    [Key]
    public int TransactionTypeId { get; set; }

    [Required]
    [StringLength(40)]
    public string Name { get; set; } = string.Empty;
}
