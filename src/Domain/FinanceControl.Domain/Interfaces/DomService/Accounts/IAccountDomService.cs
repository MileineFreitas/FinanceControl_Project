using FinanceControl.Contracts.Dtos.Accounts;
using FinanceControl.Domain.Entities.Accounts;
using FinanceControl.Domain.Interfaces.DomService.Accounts;

namespace FinanceControl.Domain.Interfaces.DomService.Accounts;

public interface IAccountDomService
{
    Account CreateFromCreateDto(AccountCreateDto dto);

    void ApplyUpdate(Account entity, AccountUpdateDto dto);

    void ValidateDelete(Guid accountId, bool hasTransactions);
}
