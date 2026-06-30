using FinanceControl.Contracts.Dtos.Common;
using FinanceControl.Contracts.Dtos.Users;
using FinanceControl.Contracts.Filters;

namespace FinanceControl.Client.Services.Interfaces.Users;

public interface IUserCliService
{
    Task<DataResultDto<UserDto>?> ListAsync(DataFilterDto? filter = null);

    Task<UserDto?> GetByIdAsync(Guid id);

    Task<HttpResponseMessage> UpdateAsync(Guid id, UserUpdateDto dto);

    Task<HttpResponseMessage> UpdateFinancialPreferencesAsync(Guid id, UserFinancialPreferencesDto dto);

    Task<Guid?> GetSecurityStampAsync(Guid id);

    Task<HttpResponseMessage> RevokeOtherSessionsAsync(Guid id);

    Task<HttpResponseMessage> DeleteAsync(Guid id);

    Task<HttpResponseMessage> DeleteAccountAsync(Guid id, string password);
}
