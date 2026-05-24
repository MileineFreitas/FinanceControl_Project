using System.Net.Http.Json;
using FinanceControl.Client.Services.Interfaces.Accounts;
using FinanceControl.Contracts.Dtos.Accounts;

namespace FinanceControl.Client.Services.Integrated.Accounts;

public sealed class AccountCliService(HttpClient httpClient) : IAccountCliService
{
    private const string BaseRoute = "api/Account";

    public async Task<IReadOnlyList<AccountDto>?> ListAsync(int? userId = null)
    {
        var query = userId.HasValue ? $"?userId={userId.Value}" : string.Empty;
        return await httpClient.GetFromJsonAsync<IReadOnlyList<AccountDto>>(BaseRoute + query);
    }

    public Task<AccountDto?> GetByIdAsync(int id) =>
        httpClient.GetFromJsonAsync<AccountDto>($"{BaseRoute}/{id}");

    public Task<HttpResponseMessage> CreateAsync(AccountCreateDto dto) =>
        httpClient.PostAsJsonAsync(BaseRoute, dto);

    public Task<HttpResponseMessage> UpdateAsync(int id, AccountUpdateDto dto) =>
        httpClient.PutAsJsonAsync($"{BaseRoute}/{id}", dto);

    public Task<HttpResponseMessage> DeleteAsync(int id) =>
        httpClient.DeleteAsync($"{BaseRoute}/{id}");
}
