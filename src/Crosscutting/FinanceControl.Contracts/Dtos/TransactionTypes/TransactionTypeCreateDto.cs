using System.ComponentModel.DataAnnotations;

namespace FinanceControl.Contracts.Dtos.TransactionTypes;

public class TransactionTypeCreateDto
{
    [Required]
    [StringLength(40)]
    public string Name { get; set; } = string.Empty;

    [StringLength(16)]
    public string Icon { get; set; } = "💳";

    [StringLength(200)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;
}
