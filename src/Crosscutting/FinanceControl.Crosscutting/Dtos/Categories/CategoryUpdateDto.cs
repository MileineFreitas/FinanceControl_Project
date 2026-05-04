using System.ComponentModel.DataAnnotations;

namespace FinanceControl.Domain.Entities;

public class CategoryUpdateDto
{
    [Required]
    public int CategoryId { get; set; }

    [Required]
    [StringLength(40)]
    public string CategoryName { get; set; } = string.Empty;

    public string? Description { get; set; }

    /// <summary>1 = Receita, 2 = Despesa, ou null para não vincular.</summary>
    public int? TransactionTypeId { get; set; }
}
