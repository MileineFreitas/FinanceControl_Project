using System.Net.Http.Json;
using FinanceControl.Client.Services.Interfaces.TransactionTypes;
using FinanceControl.Contracts.Dtos.TransactionTypes;

namespace FinanceControl.Client.Services.Integrated.TransactionTypes;

public sealed class TransactionTypeCliService(HttpClient httpClient) : ITransactionTypeCliService
{
    private const string BaseRoute = "api/TransactionTypes";

    public Task<IReadOnlyList<TransactionTypeDto>?> ListAsync(bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        var query = includeInactive ? "?includeInactive=true" : "";
        return httpClient.GetFromJsonAsync<IReadOnlyList<TransactionTypeDto>>(BaseRoute + query, cancellationToken)!;
    }

    public Task<TransactionTypeDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        httpClient.GetFromJsonAsync<TransactionTypeDto>($"{BaseRoute}/{id}", cancellationToken);

    public Task<HttpResponseMessage> CreateAsync(TransactionTypeCreateDto dto, CancellationToken cancellationToken = default) =>
        httpClient.PostAsJsonAsync(BaseRoute, dto, cancellationToken);

    public Task<HttpResponseMessage> UpdateAsync(int id, TransactionTypeUpdateDto dto, CancellationToken cancellationToken = default) =>
        httpClient.PutAsJsonAsync($"{BaseRoute}/{id}", dto, cancellationToken);

    public Task<HttpResponseMessage> DeleteAsync(int id, CancellationToken cancellationToken = default) =>
        httpClient.DeleteAsync($"{BaseRoute}/{id}", cancellationToken);
}
