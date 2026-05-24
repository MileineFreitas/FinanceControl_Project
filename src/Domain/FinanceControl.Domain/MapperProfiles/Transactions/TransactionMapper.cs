using FinanceControl.Contracts.Dtos.Transactions;
using FinanceControl.Contracts.Enumerators.Transactions;
using FinanceControl.Domain.Entities.Transactions;

namespace FinanceControl.Domain.MapperProfiles.Transactions;

public static class TransactionMapper
{
    public static TransactionDto ToDto(
        Transaction entity,
        string? categoryName = null,
        string? accountName = null,
        string? paymentMethodName = null) =>
        new()
        {
            TransactionId = entity.TransactionId,
            TransactionDescription = entity.TransactionDescription,
            TransactionValue = entity.TransactionValue,
            Date = entity.Date,
            TransactionTypeKind = entity.TransactionTypeKind,
            PaymentMethodId = entity.PaymentMethodId,
            PaymentMethodName = paymentMethodName ?? entity.PaymentMethod?.Name,
            CategoryId = entity.CategoryId,
            CategoryName = categoryName ?? entity.Category?.CategoryName,
            AccountId = entity.AccountId,
            AccountName = accountName ?? entity.Account?.Name,
            UserId = entity.UserId,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };

    public static Transaction ToEntity(TransactionCreateDto dto) =>
        new()
        {
            TransactionDescription = dto.TransactionDescription.Trim(),
            TransactionValue = dto.TransactionValue,
            Date = dto.Date,
            TransactionTypeKind = dto.TransactionTypeKind,
            PaymentMethodId = dto.PaymentMethodId,
            CategoryId = dto.CategoryId,
            AccountId = dto.AccountId,
            UserId = dto.UserId,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

    public static void ApplyUpdate(Transaction entity, TransactionUpdateDto dto)
    {
        entity.TransactionDescription = dto.TransactionDescription.Trim();
        entity.TransactionValue = dto.TransactionValue;
        entity.Date = dto.Date;
        entity.TransactionTypeKind = dto.TransactionTypeKind;
        entity.PaymentMethodId = dto.PaymentMethodId;
        entity.CategoryId = dto.CategoryId;
        entity.AccountId = dto.AccountId;
        entity.UpdatedAt = DateTimeOffset.UtcNow;
    }

    public static decimal GetBalanceDelta(decimal value, TransactionTypeKind typeKind) =>
        typeKind == TransactionTypeKind.Receita ? value : -value;
}
