namespace FinanceControl.Contracts.Interfaces.Entities.PaymentMethods;

public interface IPaymentMethod
{
    Guid PaymentMethodId { get; set; }

    string Name { get; set; }

    string Icon { get; set; }

    string? Description { get; set; }

    bool IsActive { get; set; }

    Guid? UserId { get; set; }

    DateTimeOffset DateCreated { get; set; }

    DateTimeOffset? UpdatedAt { get; set; }
}
