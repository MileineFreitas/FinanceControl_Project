using System.Net.Http.Json;
using FinanceControl.Contracts.Dtos.Auth;
using FinanceControl.Domain.Entities;

namespace FinanceControl.Web.Services;

public sealed class FinanceControlApiClient(HttpClient httpClient) : IFinanceControlApiClient
{
    private readonly HttpClient _httpClient = httpClient;

    public Task<HttpResponseMessage> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default) =>
        _httpClient.PostAsJsonAsync("user/login", request, cancellationToken);

    public Task<HttpResponseMessage> RegisterAsync(RegisterUserDto request, CancellationToken cancellationToken = default) =>
        _httpClient.PostAsJsonAsync("user/register", request, cancellationToken);

    public Task<HttpResponseMessage> RegisterCategoryAsync(CategoryRegisterDto request, CancellationToken cancellationToken = default) =>
        _httpClient.PostAsJsonAsync("category/registerCategory", request, cancellationToken);
}
