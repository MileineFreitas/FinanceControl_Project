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

    /// <summary>
    /// O provider In-Memory não aplica <see cref="ModelBuilder.HasData(object[])"/> até existir base criada.
    /// Sem <see cref="DatabaseFacade.EnsureCreated"/>, a conta padrão (AccountId=1) do modelo não existe e o seed da API falha em <c>Accounts.First(...)</c>.
    /// </summary>
    private sealed class EnsuringFinanceDbContext : FinanceDbContext
    {
        public EnsuringFinanceDbContext(DbContextOptions<FinanceDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        // Executa após Program.cs: substitui o registo do DbContext de forma consistente com o host de teste.
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<DbContextOptions<FinanceDbContext>>();
            services.RemoveAll<FinanceDbContext>();

            services.AddSingleton<DbContextOptions<FinanceDbContext>>(_ =>
            {
                var b = new DbContextOptionsBuilder<FinanceDbContext>();
                b.UseInMemoryDatabase(_inMemoryDatabaseName);
                return b.Options;
            });

            services.AddScoped<FinanceDbContext>(sp =>
                new EnsuringFinanceDbContext(sp.GetRequiredService<DbContextOptions<FinanceDbContext>>()));
        });
    }

    public HttpClient CreateApiClient()
    {
        var client = CreateClient();
        client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "application/json");
        return client;
    }
}
