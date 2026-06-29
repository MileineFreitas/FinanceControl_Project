using FinanceControl.Contracts.Currency;
using FinanceControl.Contracts.Dtos.Auth;
using FinanceControl.Contracts.Dtos.Common;
using FinanceControl.Contracts.Dtos.Users;
using FinanceControl.Contracts.Filters;
using FinanceControl.Domain.Interfaces.AppServices.Users;
using FinanceControl.Domain.Interfaces.DomService.Users;
using FinanceControl.Domain.Interfaces.Repositories.Accounts;
using FinanceControl.Domain.Interfaces.Repositories.Transactions;
using FinanceControl.Domain.Interfaces.Repositories.Users;

namespace FinanceControl.Services.Users;

public class UserAppService(
    IUserRepository repository,
    IUserDomService domService,
    IAccountRepository accountRepository,
    ITransactionRepository transactionRepository) : IUserAppService
{
    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
    {
        var user = await repository.FindByEmailAndPasswordAsync(request.Email, request.Password);
        return domService.Login(user);
    }

    public async Task<UserDto> RegisterAsync(RegisterUserDto dto)
    {
        domService.ValidateRegister(dto);
        if (await repository.EmailExistsAsync(dto.Email.Trim()))
            throw new InvalidOperationException("E-mail já cadastrado.");

        var entity = domService.CreateFromRegister(dto);
        await repository.AddAsync(entity);
        return (await repository.GetByIdAsync(entity.UserId))!;
    }

    public Task<DataResultDto<UserDto>> FilterAsync(DataFilterDto filter) =>
        repository.FilterAsync(filter);

    public Task<UserDto?> GetByIdAsync(Guid id) =>
        repository.GetByIdAsync(id);

    public async Task<UserDto?> UpdateAsync(UserUpdateDto dto)
    {
        var entity = await repository.FindTrackedAsync(dto.UserId);
        if (entity == null) return null;

        domService.ApplyUpdate(entity, dto);
        await repository.SaveChangesAsync();
        return await repository.GetByIdAsync(entity.UserId);
    }

    public async Task<UserDto?> UpdateFinancialPreferencesAsync(Guid id, UserFinancialPreferencesDto dto)
    {
        var entity = await repository.FindTrackedAsync(id);
        if (entity == null) return null;

        domService.ValidateFinancialPreferences(dto);

        var oldCurrency = entity.Currency;
        if (!string.Equals(oldCurrency, dto.Moeda, StringComparison.OrdinalIgnoreCase))
            await ConvertUserFinancialDataAsync(id, oldCurrency, dto.Moeda);

        domService.ApplyFinancialPreferences(entity, dto);
        await repository.SaveChangesAsync();
        return await repository.GetByIdAsync(entity.UserId);
    }

    private async Task ConvertUserFinancialDataAsync(Guid userId, string fromCurrency, string toCurrency)
    {
        var accounts = await accountRepository.GetTrackedByUserIdAsync(userId);
        foreach (var account in accounts)
        {
            account.InitialBalance = CurrencyConverter.Convert(account.InitialBalance, fromCurrency, toCurrency);
            account.CurrentBalance = CurrencyConverter.Convert(account.CurrentBalance, fromCurrency, toCurrency);
        }

        var transactions = await transactionRepository.GetTrackedByUserIdAsync(userId);
        foreach (var transaction in transactions)
        {
            transaction.TransactionValue = CurrencyConverter.Convert(
                transaction.TransactionValue,
                fromCurrency,
                toCurrency);
        }
    }

    public Task<bool> DeleteAsync(Guid id) =>
        repository.DeleteAsync(id);

    public async Task<bool> DeleteAccountAsync(Guid id, string password)
    {
        var entity = await repository.FindTrackedAsync(id);
        if (entity == null) return false;

        domService.ValidateDeleteAccount(entity, password);
        return await repository.DeleteAsync(id);
    }

    public Task<Guid?> GetSecurityStampAsync(Guid id) =>
        repository.GetSecurityStampAsync(id);

    public Task<Guid?> RevokeOtherSessionsAsync(Guid id) =>
        repository.RevokeOtherSessionsAsync(id);
}
