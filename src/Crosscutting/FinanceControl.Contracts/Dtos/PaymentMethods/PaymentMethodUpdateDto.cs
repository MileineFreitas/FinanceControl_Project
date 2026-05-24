using System.ComponentModel.DataAnnotations;

namespace FinanceControl.Contracts.Dtos.PaymentMethods;

public class PaymentMethodUpdateDto
{
    public Guid PaymentMethodId { get; set; }

    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(40)]
    public string Name { get; set; } = string.Empty;

    [StringLength(16)]
    public string Icon { get; set; } = "💳";

    [StringLength(200)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;
}
