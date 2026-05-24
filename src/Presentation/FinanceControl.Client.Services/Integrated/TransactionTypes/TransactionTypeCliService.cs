using System.Net.Http.Json;
using FinanceControl.Client.Services.Interfaces.TransactionTypes;
using FinanceControl.Contracts.Dtos.TransactionTypes;

namespace FinanceControl.Client.Services.Integrated.TransactionTypes;

public sealed class TransactionTypeCliService(HttpClient httpClient) : ITransactionTypeCliService
{
    private const string BaseRoute = "api/TransactionTypes";

    public Task<IReadOnlyList<TransactionTypeDto>?> ListAsync(bool includeInactive = false)
    {
        var query = includeInactive ? "?includeInactive=true" : "";
        return httpClient.GetFromJsonAsync<IReadOnlyList<TransactionTypeDto>>(BaseRoute + query)!;
    }

    public Task<TransactionTypeDto?> GetByIdAsync(int id) =>
        httpClient.GetFromJsonAsync<TransactionTypeDto>($"{BaseRoute}/{id}");

    public Task<HttpResponseMessage> CreateAsync(TransactionTypeCreateDto dto) =>
        httpClient.PostAsJsonAsync(BaseRoute, dto);

    public Task<HttpResponseMessage> UpdateAsync(int id, TransactionTypeUpdateDto dto) =>
        httpClient.PutAsJsonAsync($"{BaseRoute}/{id}", dto);

    public Task<HttpResponseMessage> DeleteAsync(int id) =>
        httpClient.DeleteAsync($"{BaseRoute}/{id}");
}
