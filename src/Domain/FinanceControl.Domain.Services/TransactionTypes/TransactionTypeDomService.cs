using FinanceControl.Contracts.Constants;
using FinanceControl.Contracts.Dtos.TransactionTypes;
using FinanceControl.Domain.Entities.TransactionTypes;
using FinanceControl.Domain.Interfaces.DomService.TransactionTypes;
using FinanceControl.Domain.MapperProfiles.TransactionTypes;

namespace FinanceControl.Domain.Services.TransactionTypes;

public class TransactionTypeDomService : ITransactionTypeDomService
{
    public TransactionTypeDefinition CreateFromDto(TransactionTypeCreateDto dto, int? userId)
    {
        if (string.IsNullOrWhiteSpace(dto.Name) || dto.Name.Trim().Length < 2)
            throw new ArgumentException("Nome deve ter pelo menos 2 caracteres.");
        if (string.IsNullOrWhiteSpace(dto.Code))
            dto.Code = PaymentMethodCodes.FromName(dto.Name);

        return TransactionTypeMapper.ToEntity(dto, userId);
    }

    public void ApplyUpdate(TransactionTypeDefinition entity, TransactionTypeUpdateDto dto)
    {
        if (entity.IsSystem && entity.Code != dto.Code.Trim().ToUpperInvariant())
            throw new InvalidOperationException("O código de tipos do sistema não pode ser alterado.");

        TransactionTypeMapper.ApplyUpdate(entity, dto);
    }

    public void EnsureCanDelete(TransactionTypeDefinition entity)
    {
        if (entity.IsSystem)
            throw new InvalidOperationException("Tipos do sistema (Débito, Crédito, Dinheiro) não podem ser excluídos.");
    }
}
