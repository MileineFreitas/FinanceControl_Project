using FinanceControl.Contracts.Dtos.Auth;
using FinanceControl.Contracts.Dtos.Common;
using FinanceControl.Contracts.Dtos.Users;
using FinanceControl.Contracts.Filters;

namespace FinanceControl.Domain.Interfaces.AppServices.Users;

public interface IUserAppService
{
    Task<DataResultDto<UserDto>> FilterAsync(DataFilterDto filter);

    Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);

    Task<UserDto?> GetByIdAsync(Guid id);

    Task<UserDto> RegisterAsync(RegisterUserDto dto);

    Task<UserDto?> UpdateAsync(UserUpdateDto dto);

    Task<bool> DeleteAsync(Guid id);
}
