using System.ComponentModel.DataAnnotations;
using FinanceControl.Contracts.Constants;

namespace FinanceControl.Contracts.Dtos.Categories;

public class CategoryUpdateDto
{
    public Guid CategoryId { get; set; }

    [Required]
    [StringLength(40)]
    public string CategoryName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string Icon { get; set; } = CategoryIcons.Default;

    public bool IsActive { get; set; } = true;
}
