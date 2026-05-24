using FinanceControl.Contracts.Dtos.TransactionTypes;
using FinanceControl.Domain.Entities.TransactionTypes;
using FinanceControl.Domain.Interfaces.DomService.TransactionTypes;
using FinanceControl.Domain.MapperProfiles.TransactionTypes;

namespace FinanceControl.Domain.Services.TransactionTypes;

public class TransactionTypeDomService : ITransactionTypeDomService
{
    public TransactionTypeDefinition CreateFromDto(TransactionTypeCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name) || dto.Name.Trim().Length < 2)
            throw new ArgumentException("Nome deve ter pelo menos 2 caracteres.");

        return TransactionTypeMapper.ToEntity(dto);
    }

    public void ApplyUpdate(TransactionTypeDefinition entity, TransactionTypeUpdateDto dto) =>
        TransactionTypeMapper.ApplyUpdate(entity, dto);

    public void EnsureCanDelete(TransactionTypeDefinition entity)
    {
        if (entity.IsSystem)
            throw new InvalidOperationException("Tipos do sistema não podem ser excluídos.");
    }
}
