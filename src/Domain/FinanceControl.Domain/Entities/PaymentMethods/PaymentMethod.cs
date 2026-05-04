using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceControl.Domain.Entities.PaymentMethods;

/// <summary>Meio de pagamento ou canal (cartão, PIX, dinheiro, etc.). Distinto do tipo de fluxo RECEITA/DESPESA usado em lançamentos.</summary>
[Table("PaymentMethods")]
public class PaymentMethod
{
    [Key]
    public int PaymentMethodId { get; set; }

    [Required]
    [StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    public DateTime DateCreated { get; set; }
}
