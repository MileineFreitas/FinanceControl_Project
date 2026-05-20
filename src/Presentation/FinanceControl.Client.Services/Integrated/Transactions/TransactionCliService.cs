using System.Net.Http.Json;
using FinanceControl.Client.Services.Interfaces.Transactions;
using FinanceControl.Contracts.Dtos.Common;
using FinanceControl.Contracts.Dtos.Transactions;
using FinanceControl.Contracts.Filters;

namespace FinanceControl.Client.Services.Integrated.Transactions;

public sealed class TransactionCliService(HttpClient httpClient) : ITransactionCliService
{
    private const string BaseRoute = "api/Transaction";

    public async Task<DataResultDto<TransactionDto>?> ListAsync(DataFilterDto? filter = null, CancellationToken cancellationToken = default)
    {
        filter ??= new DataFilterDto { Page = 1, PageSize = 200 };
        var query = $"?page={filter.Page}&pageSize={filter.PageSize}";
        if (filter.Filters != null)
        {
            foreach (var kv in filter.Filters)
                query += $"&filters[{kv.Key}]={Uri.EscapeDataString(kv.Value)}";
        }

        return await httpClient.GetFromJsonAsync<DataResultDto<TransactionDto>>(BaseRoute + query, cancellationToken);
    }

    public Task<TransactionDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        httpClient.GetFromJsonAsync<TransactionDto>($"{BaseRoute}/{id}", cancellationToken);

    public Task<HttpResponseMessage> CreateAsync(TransactionCreateDto dto, CancellationToken cancellationToken = default) =>
        httpClient.PostAsJsonAsync(BaseRoute, dto, cancellationToken);

    public Task<HttpResponseMessage> UpdateAsync(int id, TransactionUpdateDto dto, CancellationToken cancellationToken = default) =>
        httpClient.PutAsJsonAsync($"{BaseRoute}/{id}", dto, cancellationToken);

    public Task<HttpResponseMessage> DeleteAsync(int id, CancellationToken cancellationToken = default) =>
        httpClient.DeleteAsync($"{BaseRoute}/{id}", cancellationToken);
}
