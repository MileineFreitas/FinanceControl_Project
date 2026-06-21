using System.Net.Http.Json;

using FinanceControl.Client.Services.Interfaces.Transactions;

using FinanceControl.Contracts.Dtos.Common;

using FinanceControl.Contracts.Dtos.Transactions;

using FinanceControl.Contracts.Filters;



namespace FinanceControl.Client.Services.Integrated.Transactions;



public sealed class TransactionCliService(HttpClient httpClient) : ITransactionCliService

{

    private const string BaseRoute = "api/Transaction";



    public async Task<DataResultDto<TransactionDto>?> ListAsync(DataFilterDto? filter = null)

    {

        filter ??= new DataFilterDto { Page = 1, PageSize = 200 };

        var query = $"?page={filter.Page}&pageSize={filter.PageSize}";

        if (filter.Filters != null)

        {

            foreach (var kv in filter.Filters)

                query += $"&filters[{kv.Key}]={Uri.EscapeDataString(kv.Value)}";

        }



        return await httpClient.GetFromJsonAsync<DataResultDto<TransactionDto>>(BaseRoute + query);

    }



    public Task<TransactionDto?> GetByIdAsync(Guid id) =>

        httpClient.GetFromJsonAsync<TransactionDto>($"{BaseRoute}/{id}");



    public Task<HttpResponseMessage> CreateAsync(TransactionCreateDto dto) =>

        httpClient.PostAsJsonAsync(BaseRoute, dto);



    public Task<HttpResponseMessage> UpdateAsync(Guid id, TransactionUpdateDto dto) =>

        httpClient.PutAsJsonAsync($"{BaseRoute}/{id}", dto);



    public Task<HttpResponseMessage> DeleteAsync(Guid id) =>

        httpClient.DeleteAsync($"{BaseRoute}/{id}");

}

