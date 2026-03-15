using FinanceControl.Contracts.Dtos.Auth;

namespace FinanceControl.Domain.Interfaces.AppServices.Users;

public interface IUserAppService
{
    LoginResponseDto? Login(LoginRequestDto request);
}
