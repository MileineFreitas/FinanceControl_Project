using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using FinanceControl.Contracts.Constants;
using FinanceControl.Contracts.Interfaces.Entities.PaymentMethods;
using FinanceControl.Domain.Entities.Users;

namespace FinanceControl.Domain.Entities.PaymentMethods;

/// <summary>Meio de pagamento ou canal (cart�o, PIX, dinheiro, etc.). Distinto do tipo de fluxo RECEITA/DESPESA usado em lan�amentos.</summary>
[Table("PaymentMethods")]
public class PaymentMethod : IPaymentMethod

{
    [Key]
    public Guid PaymentMethodId { get; set; } = Guid.NewGuid();

    [Required]
    [StringLength(40)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(16)]
    public string Icon { get; set; } = PaymentMethodIcons.Default;

    [StringLength(200)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public Guid? UserId { get; set; }

    public DateTimeOffset DateCreated { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? UpdatedAt { get; set; }

    [JsonIgnore]
    public User? User { get; set; }
}