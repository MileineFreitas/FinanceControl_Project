using System.Net.Http.Json;
using FinanceControl.Client.Services.Interfaces.Categories;
using FinanceControl.Contracts.Dtos.Categories;
using FinanceControl.Contracts.Dtos.Common;
using FinanceControl.Contracts.Filters;

namespace FinanceControl.Client.Services.Integrated.Categories;

public sealed class CategoryCliService(HttpClient httpClient) : ICategoryCliService
{
    private const string BaseRoute = "api/Category";

    public async Task<DataResultDto<CategoryDto>?> ListAsync(DataFilterDto? filter = null, CancellationToken cancellationToken = default)
    {
        filter ??= new DataFilterDto { Page = 1, PageSize = 200 };
        var query = $"?page={filter.Page}&pageSize={filter.PageSize}";
        if (filter.Filters != null)
        {
            foreach (var kv in filter.Filters)
                query += $"&filters[{kv.Key}]={Uri.EscapeDataString(kv.Value)}";
        }

        return await httpClient.GetFromJsonAsync<DataResultDto<CategoryDto>>(BaseRoute + query, cancellationToken);
    }

    public Task<CategoryDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        httpClient.GetFromJsonAsync<CategoryDto>($"{BaseRoute}/{id}", cancellationToken);

    public Task<HttpResponseMessage> CreateAsync(CategoryRegisterDto dto, CancellationToken cancellationToken = default) =>
        httpClient.PostAsJsonAsync($"{BaseRoute}/registerCategory", dto, cancellationToken);

    public Task<HttpResponseMessage> UpdateAsync(int id, CategoryUpdateDto dto, CancellationToken cancellationToken = default) =>
        httpClient.PutAsJsonAsync($"{BaseRoute}/{id}", dto, cancellationToken);

    public Task<HttpResponseMessage> DeleteAsync(int id, CancellationToken cancellationToken = default) =>
        httpClient.DeleteAsync($"{BaseRoute}/{id}", cancellationToken);
}
