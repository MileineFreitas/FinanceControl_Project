using FinanceControl.Contracts.Dtos.Auth;
using FinanceControl.Contracts.Dtos.Common;
using FinanceControl.Contracts.Dtos.Users;
using FinanceControl.Contracts.Filters;

namespace FinanceControl.Client.Services.Interfaces.Users;

public interface IUserCliService
{
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);

    Task<HttpResponseMessage> RegisterAsync(RegisterUserDto dto, CancellationToken cancellationToken = default);

    Task<DataResultDto<UserDto>?> ListAsync(DataFilterDto? filter = null, CancellationToken cancellationToken = default);

    Task<UserDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<HttpResponseMessage> UpdateAsync(int id, UserUpdateDto dto, CancellationToken cancellationToken = default);

    Task<HttpResponseMessage> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
