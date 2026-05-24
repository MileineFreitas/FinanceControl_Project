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
            Icon = PaymentMethodIcons.Normalize(entity.Icon),
            Description = entity.Description,
            IsSystem = entity.IsSystem,
            IsActive = entity.IsActive
        };

    public static TransactionTypeDefinition ToEntity(TransactionTypeCreateDto dto) =>
        new()
        {
            Name = dto.Name.Trim(),
            Icon = PaymentMethodIcons.Normalize(dto.Icon),
            Description = dto.Description?.Trim(),
            IsSystem = false,
            IsActive = dto.IsActive
        };

    public static void ApplyUpdate(TransactionTypeDefinition entity, TransactionTypeUpdateDto dto)
    {
        entity.Name = dto.Name.Trim();
        entity.Icon = PaymentMethodIcons.Normalize(dto.Icon);
        entity.Description = dto.Description?.Trim();
        entity.IsActive = dto.IsActive;
    }
}
