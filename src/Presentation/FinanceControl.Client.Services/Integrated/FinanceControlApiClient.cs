using System.Net.Http.Json;
using FinanceControl.Client.Services.Interfaces;
using FinanceControl.Contracts.Dtos.Auth;
using FinanceControl.Contracts.Dtos.Users;

namespace FinanceControl.Client.Services.Integrated;

public sealed class FinanceControlApiClient(HttpClient httpClient) : IFinanceControlApiClient
{
    public Task<HttpResponseMessage> LoginAsync(LoginRequestDto request) =>
        httpClient.PostAsJsonAsync("User/login", request);

    public Task<HttpResponseMessage> RegisterAsync(RegisterUserDto request) =>
        httpClient.PostAsJsonAsync("User/register", request);
}
