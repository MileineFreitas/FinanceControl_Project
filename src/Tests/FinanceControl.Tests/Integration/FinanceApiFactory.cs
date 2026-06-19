using FinanceControl.Infrastructure.Contexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FinanceControl.Tests.Integration;

/// <summary>
/// Host da API com MySQL substituído por banco em memória (sem dependência externa nos testes).
/// </summary>
public class FinanceApiFactory : WebApplicationFactory<Program>, IDisposable
{
    /// <summary>Um único nome de BD por instância da factory, alinhado ao IServiceProvider usado em requests e em CreateScope().</summary>
    private readonly string _inMemoryDatabaseName = $"FinanceTests_{Guid.NewGuid():N}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        // Executa após Program.cs: substitui o registo do DbContext de forma consistente com o host de teste.
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<DbContextOptions<FinanceDbContext>>();
            services.RemoveAll<FinanceDbContext>();

            services.AddDbContext<FinanceDbContext>(options =>
                options.UseInMemoryDatabase(_inMemoryDatabaseName));
        });
    }

    public HttpClient CreateApiClient()
    {
        var client = CreateClient();
        client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "application/json");
        return client;
    }
}
