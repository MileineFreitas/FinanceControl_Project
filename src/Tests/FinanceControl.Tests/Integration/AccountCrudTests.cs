using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace FinanceControl.Tests.Integration;

/// <summary>Testes individuais do CRUD de contas (api/Account).</summary>
public class AccountCrudTests : IClassFixture<FinanceApiFactory>
{
    private readonly FinanceApiFactory _factory;
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public AccountCrudTests(FinanceApiFactory factory) => _factory = factory;

    [Fact]
    public async Task Get_All_ReturnsOk_AndJsonArray()
    {
        using var client = _factory.CreateApiClient();
        var res = await client.GetAsync("/api/Account");
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        var list = await res.Content.ReadFromJsonAsync<JsonElement>(JsonOpts);
        Assert.Equal(JsonValueKind.Array, list.ValueKind);
    }

    [Fact]
    public async Task Post_Create_ReturnsCreated_WithId()
    {
        using var client = _factory.CreateApiClient();
        var body = new { name = $"Conta_{Guid.NewGuid():N}", initialBalance = 100m, userId = (Guid?)null };
        var res = await client.PostAsJsonAsync("/api/Account", body);
        Assert.Equal(HttpStatusCode.Created, res.StatusCode);
        var id = JsonResponse.GetIdFromCreatedLocation(res);
        Assert.NotEqual(Guid.Empty, id);
    }

    [Fact]
    public async Task Get_ById_ReturnsOk_WhenExists()
    {
        using var client = _factory.CreateApiClient();
        var create = await client.PostAsJsonAsync("/api/Account", new { name = "GetById", initialBalance = 0m, userId = (Guid?)null });
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        var id = JsonResponse.GetIdFromCreatedLocation(create);

        var res = await client.GetAsync($"/api/Account/{id}");
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
    }

    [Fact]
    public async Task Put_Update_ReturnsNoContent()
    {
        using var client = _factory.CreateApiClient();
        var create = await client.PostAsJsonAsync("/api/Account", new { name = "PutTest", initialBalance = 50m, userId = (Guid?)null });
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        var id = JsonResponse.GetIdFromCreatedLocation(create);

        var putBody = new { accountId = id, name = "PutTest_Editado", initialBalance = 50m, currentBalance = 45m };
        var res = await client.PutAsJsonAsync($"/api/Account/{id}", putBody);
        Assert.Equal(HttpStatusCode.NoContent, res.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenNoTransactions()
    {
        using var client = _factory.CreateApiClient();
        var create = await client.PostAsJsonAsync("/api/Account", new { name = "ToDelete", initialBalance = 0m, userId = (Guid?)null });
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        var id = JsonResponse.GetIdFromCreatedLocation(create);

        var res = await client.DeleteAsync($"/api/Account/{id}");
        Assert.Equal(HttpStatusCode.NoContent, res.StatusCode);
    }
}
