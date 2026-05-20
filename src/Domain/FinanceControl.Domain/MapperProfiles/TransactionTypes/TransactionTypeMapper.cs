using FinanceControl.Contracts.Constants;
using FinanceControl.Contracts.Dtos.TransactionTypes;
using FinanceControl.Domain.Entities.TransactionTypes;

namespace FinanceControl.Domain.MapperProfiles.TransactionTypes;

public static class TransactionTypeMapper
{
    public static TransactionTypeDto ToDto(TransactionTypeDefinition entity) =>
        new()
        {
            TransactionTypeId = entity.TransactionTypeId,
            Name = entity.Name,
            Code = entity.Code,
            Icon = PaymentMethodIcons.Normalize(entity.Icon),
            PaymentKind = entity.PaymentKind,
            Description = entity.Description,
            IsSystem = entity.IsSystem,
            IsActive = entity.IsActive
        };

    public static TransactionTypeDefinition ToEntity(TransactionTypeCreateDto dto, int? userId) =>
        new()
        {
            Name = dto.Name.Trim(),
            Code = dto.Code.Trim().ToUpperInvariant(),
            Icon = PaymentMethodIcons.Normalize(dto.Icon),
            PaymentKind = dto.PaymentKind,
            Description = dto.Description?.Trim(),
            IsSystem = false,
            IsActive = dto.IsActive,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

    public static void ApplyUpdate(TransactionTypeDefinition entity, TransactionTypeUpdateDto dto)
    {
        entity.Name = dto.Name.Trim();
        entity.Code = dto.Code.Trim().ToUpperInvariant();
        entity.Icon = PaymentMethodIcons.Normalize(dto.Icon);
        entity.PaymentKind = dto.PaymentKind;
        entity.Description = dto.Description?.Trim();
        entity.IsActive = dto.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;
    }
}
