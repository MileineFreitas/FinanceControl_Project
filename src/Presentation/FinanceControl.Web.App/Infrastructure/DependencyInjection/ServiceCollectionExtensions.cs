using FinanceControl.Web.Options;
using FinanceControl.Web.Services;
using Microsoft.Extensions.Options;

namespace FinanceControl.Web.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFinanceControlApiClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ApiClientOptions>(configuration.GetSection(ApiClientOptions.SectionName));

        services.AddHttpClient<IFinanceControlApiClient, FinanceControlApiClient>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<ApiClientOptions>>().Value;
            var baseUrl = options.BaseUrl.TrimEnd('/');
            client.BaseAddress = new Uri(baseUrl + "/");
            client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
            client.Timeout = TimeSpan.FromSeconds(Math.Max(1, options.TimeoutSeconds));
        });

        return services;
    }
}
