using FinanceControl.Contracts.Constants;
using FinanceControl.Contracts.Dtos.PaymentMethods;
using FinanceControl.Domain.Entities.PaymentMethods;

namespace FinanceControl.Domain.MapperProfiles.PaymentMethods;

public static class PaymentMethodMapper
{
    public static PaymentMethodDto ToDto(PaymentMethod entity) =>
        new()
        {
            PaymentMethodId = entity.PaymentMethodId,
            Name = entity.Name,
            Icon = PaymentMethodIcons.Normalize(entity.Icon),
            Description = entity.Description,
            IsActive = entity.IsActive,
            UserId = entity.UserId,
            DateCreated = entity.DateCreated,
            UpdatedAt = entity.UpdatedAt
        };

    public static PaymentMethod ToEntity(PaymentMethodCreateDto dto) =>
        new()
        {
            Name = dto.Name.Trim(),
            Icon = PaymentMethodIcons.Normalize(dto.Icon),
            Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim(),
            IsActive = dto.IsActive,
            UserId = dto.UserId,
            DateCreated = DateTimeOffset.UtcNow
        };

    public static void ApplyUpdate(PaymentMethod entity, PaymentMethodUpdateDto dto)
    {
        entity.Name = dto.Name.Trim();
        entity.Icon = PaymentMethodIcons.Normalize(dto.Icon);
        entity.Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim();
        entity.IsActive = dto.IsActive;
        entity.UpdatedAt = DateTimeOffset.UtcNow;
    }
}
