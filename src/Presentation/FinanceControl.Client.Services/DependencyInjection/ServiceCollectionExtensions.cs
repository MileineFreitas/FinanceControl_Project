using FinanceControl.Client.Services.Integrated;
using FinanceControl.Client.Services.Integrated.Accounts;
using FinanceControl.Client.Services.Integrated.Categories;
using FinanceControl.Client.Services.Integrated.PaymentMethods;
using FinanceControl.Client.Services.Integrated.Transactions;
using FinanceControl.Client.Services.Integrated.Users;
using FinanceControl.Client.Services.Interfaces;
using FinanceControl.Client.Services.Interfaces.Accounts;
using FinanceControl.Client.Services.Interfaces.Categories;
using FinanceControl.Client.Services.Interfaces.PaymentMethods;
using FinanceControl.Client.Services.Interfaces.Transactions;
using FinanceControl.Client.Services.Interfaces.Users;
using FinanceControl.Client.Services.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FinanceControl.Client.Services.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFinanceControlClientServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ApiClientOptions>(configuration.GetSection(ApiClientOptions.SectionName));

        services.AddHttpClient<IFinanceControlApiClient, FinanceControlApiClient>(ConfigureClient);
        services.AddHttpClient<ICategoryCliService, CategoryCliService>(ConfigureClient);
        services.AddHttpClient<ITransactionCliService, TransactionCliService>(ConfigureClient);
        services.AddHttpClient<IAccountCliService, AccountCliService>(ConfigureClient);
        services.AddHttpClient<IUserCliService, UserCliService>(ConfigureClient);
        services.AddHttpClient<IPaymentMethodCliService, PaymentMethodCliService>(ConfigureClient);

        return services;
    }

    private static void ConfigureClient(IServiceProvider sp, HttpClient client)
    {
        var options = sp.GetRequiredService<IOptions<ApiClientOptions>>().Value;
        var baseUrl = options.BaseUrl.TrimEnd('/');
        client.BaseAddress = new Uri(baseUrl + "/");
        client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
        client.Timeout = TimeSpan.FromSeconds(Math.Max(1, options.TimeoutSeconds));
    }
}
