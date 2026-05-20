using System.Net.Http.Json;
using FinanceControl.Client.Services.Interfaces.Users;
using FinanceControl.Contracts.Dtos.Auth;
using FinanceControl.Contracts.Dtos.Common;
using FinanceControl.Contracts.Dtos.Users;
using FinanceControl.Contracts.Filters;

namespace FinanceControl.Client.Services.Integrated.Users;

public sealed class UserCliService(HttpClient httpClient) : IUserCliService
{
    private const string BaseRoute = "User";

    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync($"{BaseRoute}/login", request, cancellationToken);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<LoginResponseDto>(cancellationToken: cancellationToken);
    }

    public Task<HttpResponseMessage> RegisterAsync(RegisterUserDto dto, CancellationToken cancellationToken = default) =>
        httpClient.PostAsJsonAsync($"{BaseRoute}/register", dto, cancellationToken);

    public async Task<DataResultDto<UserDto>?> ListAsync(DataFilterDto? filter = null, CancellationToken cancellationToken = default)
    {
        filter ??= new DataFilterDto { Page = 1, PageSize = 200 };
        var query = $"?page={filter.Page}&pageSize={filter.PageSize}";
        return await httpClient.GetFromJsonAsync<DataResultDto<UserDto>>(BaseRoute + query, cancellationToken);
    }

    public Task<UserDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        httpClient.GetFromJsonAsync<UserDto>($"{BaseRoute}/{id}", cancellationToken);

    public Task<HttpResponseMessage> UpdateAsync(int id, UserUpdateDto dto, CancellationToken cancellationToken = default) =>
        httpClient.PutAsJsonAsync($"{BaseRoute}/{id}/user-update", dto, cancellationToken);

    public Task<HttpResponseMessage> DeleteAsync(int id, CancellationToken cancellationToken = default) =>
        httpClient.DeleteAsync($"{BaseRoute}/{id}", cancellationToken);
}
