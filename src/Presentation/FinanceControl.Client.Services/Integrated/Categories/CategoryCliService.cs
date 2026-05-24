using System.Net.Http.Json;
using FinanceControl.Client.Services.Interfaces.Categories;
using FinanceControl.Contracts.Dtos.Categories;
using FinanceControl.Contracts.Dtos.Common;
using FinanceControl.Contracts.Filters;

namespace FinanceControl.Client.Services.Integrated.Categories;

public sealed class CategoryCliService(HttpClient httpClient) : ICategoryCliService
{
    private const string BaseRoute = "api/Category";

    public async Task<DataResultDto<CategoryDto>?> ListAsync(DataFilterDto? filter = null)
    {
        filter ??= new DataFilterDto { Page = 1, PageSize = 200 };
        var query = $"?page={filter.Page}&pageSize={filter.PageSize}";
        if (filter.Filters != null)
        {
            foreach (var kv in filter.Filters)
                query += $"&filters[{kv.Key}]={Uri.EscapeDataString(kv.Value)}";
        }

        return await httpClient.GetFromJsonAsync<DataResultDto<CategoryDto>>(BaseRoute + query);
    }

    public Task<CategoryDto?> GetByIdAsync(int id) =>
        httpClient.GetFromJsonAsync<CategoryDto>($"{BaseRoute}/{id}");

    public Task<HttpResponseMessage> CreateAsync(CategoryRegisterDto dto) =>
        httpClient.PostAsJsonAsync($"{BaseRoute}/registerCategory", dto);

    public Task<HttpResponseMessage> UpdateAsync(int id, CategoryUpdateDto dto) =>
        httpClient.PutAsJsonAsync($"{BaseRoute}/{id}", dto);

    public Task<HttpResponseMessage> DeleteAsync(int id) =>
        httpClient.DeleteAsync($"{BaseRoute}/{id}");
}
