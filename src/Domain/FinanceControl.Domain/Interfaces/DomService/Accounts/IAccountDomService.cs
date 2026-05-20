using FinanceControl.Contracts.Dtos.Accounts;
using FinanceControl.Domain.Entities.Accounts;

namespace FinanceControl.Domain.Interfaces.DomService.Accounts;

public interface IAccountDomService
{
    Account CreateFromCreateDto(AccountCreateDto dto);

    void ApplyUpdate(Account entity, AccountUpdateDto dto);

    void ValidateDelete(int accountId, bool hasTransactions);
}
