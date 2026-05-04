using System.ComponentModel.DataAnnotations;

namespace FinanceControl.Domain.Entities;

/// <summary>Cadastro de meio de pagamento (nome + descrição opcional).</summary>
public class PaymentMethodRegisterDto
{
    [Required(ErrorMessage = "Nome é obrigatório.")]
    [MinLength(2, ErrorMessage = "Informe pelo menos 2 caracteres.")]
    [StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string Description { get; set; } = string.Empty;
}
