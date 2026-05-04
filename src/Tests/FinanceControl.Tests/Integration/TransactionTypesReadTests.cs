using System.Net;
using System.Text.Json;

namespace FinanceControl.Tests.Integration;

/// <summary>Leitura dos tipos de transação (sem CRUD de escrita — dados seed).</summary>
public class TransactionTypesReadTests : IClassFixture<FinanceApiFactory>
{
    private readonly FinanceApiFactory _factory;

    public TransactionTypesReadTests(FinanceApiFactory factory) => _factory = factory;

    [Fact]
    public async Task Get_ReturnsOk_WithReceitaEDespesa()
    {
        await IntegrationSeed.EnsureDatabaseAndSeedAsync(_factory.Services);
        using var client = _factory.CreateApiClient();
        var res = await client.GetAsync("/api/TransactionTypes");
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);

        var json = await res.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;
        Assert.Equal(JsonValueKind.Array, root.ValueKind);
        Assert.True(root.GetArrayLength() >= 2);
    }
}
