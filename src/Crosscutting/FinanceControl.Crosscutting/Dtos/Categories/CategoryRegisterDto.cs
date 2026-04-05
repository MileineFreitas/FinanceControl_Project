using System.ComponentModel.DataAnnotations;
using FinanceControl.Domain.Enums;

namespace FinanceControl.Domain.Entities;

public class CategoryRegisterDto
{
    [Required(ErrorMessage = "Nome é obrigatorio.")]
    [MinLength(2, ErrorMessage = "Minimo 3 caracteres")]
    public string CategoryName { get; set; } = string.Empty;

    public string CategoryDescription { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tipo (Receita ou Despesa) é obrigatório.")]
    public TransactionType Type { get; set; }

    
}
