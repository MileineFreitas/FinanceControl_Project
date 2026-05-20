using FinanceControl.Contracts.Dtos.Accounts;
using FinanceControl.Domain.Entities.Accounts;

namespace FinanceControl.Domain.MapperProfiles.Accounts;

public static class AccountMapper
{
    public static AccountDto ToDto(Account entity) =>
        new()
        {
            AccountId = entity.AccountId,
            Name = entity.Name,
            InitialBalance = entity.InitialBalance,
            CurrentBalance = entity.CurrentBalance,
            CreatedAt = entity.CreatedAt,
            UserId = entity.UserId
        };

    public static Account ToEntity(AccountCreateDto dto) =>
        new()
        {
            Name = dto.Name.Trim(),
            InitialBalance = dto.InitialBalance,
            CurrentBalance = dto.InitialBalance,
            CreatedAt = DateTime.UtcNow,
            UserId = dto.UserId
        };

    public static void ApplyUpdate(Account entity, AccountUpdateDto dto)
    {
        entity.Name = dto.Name.Trim();
        entity.InitialBalance = dto.InitialBalance;
        entity.CurrentBalance = dto.CurrentBalance;
    }
}
