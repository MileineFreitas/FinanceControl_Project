using System.ComponentModel.DataAnnotations;
using FinanceControl.Contracts.Constants;

namespace FinanceControl.Contracts.Dtos.Categories;

public class CategoryRegisterDto
{
    [Required(ErrorMessage = "Nome é obrigatório.")]
    [MinLength(2, ErrorMessage = "Mínimo 2 caracteres.")]
    public string CategoryName { get; set; } = string.Empty;

    public string? CategoryDescription { get; set; }

    public string Icon { get; set; } = CategoryIcons.Default;

    public bool IsActive { get; set; } = true;
}
