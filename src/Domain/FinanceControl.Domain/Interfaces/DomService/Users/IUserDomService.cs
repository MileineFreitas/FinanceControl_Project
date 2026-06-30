using FinanceControl.Contracts.Dtos.Auth;
using FinanceControl.Contracts.Dtos.Users;
using FinanceControl.Domain.Entities.Users;

namespace FinanceControl.Domain.Interfaces.DomService.Users;

public interface IUserDomService
{
    LoginResponseDto? Login(User? user);

    User CreateFromRegister(UserRegisterDto dto);

    void ApplyUpdate(User entity, UserUpdateDto dto);

    void ApplyFinancialPreferences(User entity, UserFinancialPreferencesDto dto);

    void ValidateFinancialPreferences(UserFinancialPreferencesDto dto);

    void ValidateRegister(UserRegisterDto dto);

    void ValidateDeleteAccount(User entity, string password);
}
