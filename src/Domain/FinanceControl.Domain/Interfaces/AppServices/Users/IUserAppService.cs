using FinanceControl.Contracts.Dtos.Auth;
using FinanceControl.Contracts.Dtos.Common;
using FinanceControl.Contracts.Dtos.Users;
using FinanceControl.Contracts.Filters;

namespace FinanceControl.Domain.Interfaces.AppServices.Users;

public interface IUserAppService
{
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);

    Task<UserDto> RegisterAsync(UserRegisterDto dto);

    Task<DataResultDto<UserDto>> FilterAsync(DataFilterDto filter);

    Task<UserDto?> GetByIdAsync(int id);

    Task<UserDto?> UpdateAsync(UserUpdateDto dto);

    Task<bool> DeleteAsync(int id);
}
