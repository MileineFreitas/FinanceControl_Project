using FinanceControl.Contracts.Dtos.Transactions;
using FinanceControl.Contracts.Enumerators.Transactions;
using FinanceControl.Domain.Entities.Transactions;
using FinanceControl.Domain.Interfaces.DomService.Transactions;
using FinanceControl.Domain.MapperProfiles.Transactions;

namespace FinanceControl.Domain.Services.Transactions;

public class TransactionDomService : ITransactionDomService
{
    public Transaction CreateFromCreateDto(TransactionCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.TransactionDescription))
            throw new ArgumentException("Descrição da transação é obrigatória.");
        if (dto.TransactionValue <= 0)
            throw new ArgumentException("Valor da transação deve ser maior que zero.");
        if (!dto.TransactionTypeKind.IsDefinedKind())
            throw new ArgumentException("Tipo de transação inválido (receita ou despesa).");
        if (dto.PaymentKind is { } pk && !Enum.IsDefined(typeof(PaymentKind), pk))
            throw new ArgumentException("Meio de pagamento inválido.");

        return TransactionMapper.ToEntity(dto);
    }

    public void ApplyUpdate(Transaction entity, TransactionUpdateDto dto) =>
        TransactionMapper.ApplyUpdate(entity, dto);

    public decimal GetBalanceDelta(decimal value, TransactionTypeKind typeKind) =>
        TransactionMapper.GetBalanceDelta(value, typeKind);
}
