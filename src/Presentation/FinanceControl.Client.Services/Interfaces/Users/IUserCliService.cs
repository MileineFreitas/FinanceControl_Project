using FinanceControl.Contracts.Dtos.Auth;
using FinanceControl.Contracts.Dtos.Common;
using FinanceControl.Contracts.Dtos.Users;
using FinanceControl.Contracts.Filters;

namespace FinanceControl.Client.Services.Interfaces.Users;

public interface IUserCliService
{
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);

    Task<HttpResponseMessage> RegisterAsync(RegisterUserDto dto);

    Task<DataResultDto<UserDto>?> ListAsync(DataFilterDto? filter = null);

    Task<UserDto?> GetByIdAsync(int id);

    Task<HttpResponseMessage> UpdateAsync(int id, UserUpdateDto dto);

    Task<HttpResponseMessage> DeleteAsync(int id);
}
