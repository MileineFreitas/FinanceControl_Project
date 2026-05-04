using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace FinanceControl.Tests.Integration;

/// <summary>Testes individuais do CRUD de categorias (api/Category).</summary>
public class CategoryCrudTests : IClassFixture<FinanceApiFactory>
{
    private readonly FinanceApiFactory _factory;
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public CategoryCrudTests(FinanceApiFactory factory) => _factory = factory;

    [Fact]
    public async Task Get_All_ReturnsOk()
    {
        using var client = _factory.CreateApiClient();
        var res = await client.GetAsync("/api/Category");
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
    }

    [Fact]
    public async Task Register_ReturnsCreated()
    {
        using var client = _factory.CreateApiClient();
        var body = new
        {
            categoryName = $"Cat_{Guid.NewGuid():N}",
            categoryDescription = "teste",
            type = 2
        };
        var res = await client.PostAsJsonAsync("/api/Category/registerCategory", body);
        Assert.Equal(HttpStatusCode.Created, res.StatusCode);
    }

    [Fact]
    public async Task Put_Update_ReturnsNoContent()
    {
        using var client = _factory.CreateApiClient();
        var reg = await client.PostAsJsonAsync("/api/Category/registerCategory", new
        {
            categoryName = $"PutCat_{Guid.NewGuid():N}",
            categoryDescription = "x",
            type = 1
        });
        var json = await reg.Content.ReadFromJsonAsync<JsonElement>(JsonOpts);
        var id = JsonResponse.GetInt32(json, "categoryId", "CategoryId");

        var put = new { categoryId = id, categoryName = "Editada", description = "d", transactionTypeId = 1 };
        var res = await client.PutAsJsonAsync($"/api/Category/{id}", put);
        Assert.Equal(HttpStatusCode.NoContent, res.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsOk()
    {
        using var client = _factory.CreateApiClient();
        var reg = await client.PostAsJsonAsync("/api/Category/registerCategory", new
        {
            categoryName = $"DelCat_{Guid.NewGuid():N}",
            categoryDescription = "y",
            type = 2
        });
        var json = await reg.Content.ReadFromJsonAsync<JsonElement>(JsonOpts);
        var id = JsonResponse.GetInt32(json, "categoryId", "CategoryId");

        var res = await client.DeleteAsync($"/api/Category/{id}");
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
    }
}
