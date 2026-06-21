using FinanceControl.Contracts.Dtos.Common;
using FinanceControl.Contracts.Dtos.Users;
using FinanceControl.Contracts.Filters;

namespace FinanceControl.Client.Services.Interfaces.Users;

public interface IUserCliService
{
    Task<DataResultDto<UserDto>?> ListAsync(DataFilterDto? filter = null);

    Task<UserDto?> GetByIdAsync(Guid id);

    Task<HttpResponseMessage> UpdateAsync(Guid id, UserUpdateDto dto);
}
