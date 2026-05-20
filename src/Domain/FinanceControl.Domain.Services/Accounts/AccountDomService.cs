using FinanceControl.Contracts.Dtos.Accounts;
using FinanceControl.Domain.Entities.Accounts;
using FinanceControl.Domain.Interfaces.DomService.Accounts;
using FinanceControl.Domain.MapperProfiles.Accounts;

namespace FinanceControl.Domain.Services.Accounts;

public class AccountDomService : IAccountDomService
{
    public Account CreateFromCreateDto(AccountCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name) || dto.Name.Trim().Length < 2)
            throw new ArgumentException("Nome da conta deve ter pelo menos 2 caracteres.");

        return AccountMapper.ToEntity(dto);
    }

    public void ApplyUpdate(Account entity, AccountUpdateDto dto) =>
        AccountMapper.ApplyUpdate(entity, dto);

    public void ValidateDelete(int accountId, bool hasTransactions)
    {
        if (accountId == 1)
            throw new InvalidOperationException("A conta padrão (Principal) não pode ser excluída.");
        if (hasTransactions)
            throw new InvalidOperationException("Não é possível excluir: existem transações vinculadas a esta conta.");
    }
}
