using System.Net.Http.Json;
using FinanceControl.Client.Services.Interfaces.Accounts;
using FinanceControl.Contracts.Dtos.Accounts;

namespace FinanceControl.Client.Services.Integrated.Accounts;

public sealed class AccountCliService(HttpClient httpClient) : IAccountCliService
{
    private const string BaseRoute = "api/Account";

    public async Task<IReadOnlyList<AccountDto>?> ListAsync(int? userId = null, CancellationToken cancellationToken = default)
    {
        var query = userId.HasValue ? $"?userId={userId.Value}" : string.Empty;
        return await httpClient.GetFromJsonAsync<IReadOnlyList<AccountDto>>(BaseRoute + query, cancellationToken);
    }

    public Task<AccountDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        httpClient.GetFromJsonAsync<AccountDto>($"{BaseRoute}/{id}", cancellationToken);

    public Task<HttpResponseMessage> CreateAsync(AccountCreateDto dto, CancellationToken cancellationToken = default) =>
        httpClient.PostAsJsonAsync(BaseRoute, dto, cancellationToken);

    public Task<HttpResponseMessage> UpdateAsync(int id, AccountUpdateDto dto, CancellationToken cancellationToken = default) =>
        httpClient.PutAsJsonAsync($"{BaseRoute}/{id}", dto, cancellationToken);

    public Task<HttpResponseMessage> DeleteAsync(int id, CancellationToken cancellationToken = default) =>
        httpClient.DeleteAsync($"{BaseRoute}/{id}", cancellationToken);
}
