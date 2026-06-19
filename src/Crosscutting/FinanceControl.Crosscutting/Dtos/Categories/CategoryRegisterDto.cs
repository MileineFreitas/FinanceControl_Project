using System.ComponentModel.DataAnnotations;
using FinanceControl.Domain.Enums;

namespace FinanceControl.Domain.Entities;

public class CategoryRegisterDto
{
    [Required(ErrorMessage = "Nome é obrigatorio.")]
    [MinLength(2, ErrorMessage = "Minimo 3 caracteres")]
    public string CategoryName { get; set; } = string.Empty;

    public string CategoryDescription { get; set; } = string.Empty;

    /// <summary>Tipo de fluxo na API legada; a UI de categorias nao expoe mais esta escolha (usa padrao ao gravar).</summary>
    public TransactionType Type { get; set; } = TransactionType.Despesa;
}

