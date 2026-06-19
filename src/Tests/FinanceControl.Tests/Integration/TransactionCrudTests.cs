using System.Net;
using System.Net.Http.Json;

namespace FinanceControl.Tests.Integration;

/// <summary>Testes individuais do CRUD de transações (api/Transaction).</summary>
public class TransactionCrudTests : IClassFixture<FinanceApiFactory>
{
    private readonly FinanceApiFactory _factory;

    public TransactionCrudTests(FinanceApiFactory factory) => _factory = factory;

    [Fact]
    public async Task Get_List_ReturnsOk()
    {
        using var client = _factory.CreateApiClient();
        var res = await client.GetAsync("/api/Transaction");
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
    }

    [Fact]
    public async Task Post_Then_GetById_Then_Put_Then_Delete_Flow()
    {
        using var client = _factory.CreateApiClient();
        var (userId, categoryId, accountId, paymentMethodId) = await IntegrationSeed.EnsureUserAndCategoryAsync(_factory.Services);

        var createBody = new
        {
            transactionDescription = "Teste integração",
            transactionValue = 10.5m,
            date = DateTime.UtcNow,
            transactionTypeKind = 2,
            paymentMethodId,
            categoryId,
            accountId,
            userId,
            status = 2
        };

        var post = await client.PostAsJsonAsync("/api/Transaction", createBody);
        Assert.Equal(HttpStatusCode.Created, post.StatusCode);
        var txId = JsonResponse.GetIdFromCreatedLocation(post);

        var getOne = await client.GetAsync($"/api/Transaction/{txId}");
        Assert.Equal(HttpStatusCode.OK, getOne.StatusCode);

        var putBody = new
        {
            transactionId = txId,
            transactionDescription = "Teste integração (editado)",
            transactionValue = 11m,
            date = DateTime.UtcNow,
            transactionTypeKind = 2,
            paymentMethodId,
            categoryId,
            accountId,
            status = 2
        };
        var put = await client.PutAsJsonAsync($"/api/Transaction/{txId}", putBody);
        Assert.Equal(HttpStatusCode.NoContent, put.StatusCode);

        var del = await client.DeleteAsync($"/api/Transaction/{txId}");
        Assert.Equal(HttpStatusCode.NoContent, del.StatusCode);

        var getMissing = await client.GetAsync($"/api/Transaction/{txId}");
        Assert.Equal(HttpStatusCode.NotFound, getMissing.StatusCode);
    }

    [Fact]
    public async Task Get_FilterByUserId_ReturnsOk()
    {
        using var client = _factory.CreateApiClient();
        var (userId, _, _, _) = await IntegrationSeed.EnsureUserAndCategoryAsync(_factory.Services);
        var res = await client.GetAsync($"/api/Transaction?userId={userId}");
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
    }
}
