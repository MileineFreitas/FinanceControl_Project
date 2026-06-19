namespace FinanceControl.Contracts.Dtos.PaymentMethods;

public class PaymentMethodDto
{
    public Guid PaymentMethodId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Icon { get; set; } = "💳";

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public Guid? UserId { get; set; }

    public DateTimeOffset DateCreated { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }
}
