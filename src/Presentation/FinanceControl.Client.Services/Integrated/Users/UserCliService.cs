using System.Net.Http.Json;
using FinanceControl.Client.Services.Interfaces.Users;
using FinanceControl.Contracts.Dtos.Common;
using FinanceControl.Contracts.Dtos.Users;
using FinanceControl.Contracts.Filters;

namespace FinanceControl.Client.Services.Integrated.Users;

public sealed class UserCliService(HttpClient httpClient) : IUserCliService
{
    private const string BaseRoute = "User";

    public async Task<DataResultDto<UserDto>?> ListAsync(DataFilterDto? filter = null)
    {
        filter ??= new DataFilterDto { Page = 1, PageSize = 200 };
        var query = $"?page={filter.Page}&pageSize={filter.PageSize}";
        return await httpClient.GetFromJsonAsync<DataResultDto<UserDto>>(BaseRoute + query);
    }

    public Task<UserDto?> GetByIdAsync(Guid id) =>
        httpClient.GetFromJsonAsync<UserDto>($"{BaseRoute}/{id}");

    public Task<HttpResponseMessage> UpdateAsync(Guid id, UserUpdateDto dto) =>
        httpClient.PutAsJsonAsync($"{BaseRoute}/{id}/user-update", dto);
}
