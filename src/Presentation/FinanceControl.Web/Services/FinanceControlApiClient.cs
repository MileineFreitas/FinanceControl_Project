using System.Net.Http.Json;
using FinanceControl.Contracts.Dtos.Auth;
using FinanceControl.Domain.Entities;

namespace FinanceControl.Web.Services;

public sealed class FinanceControlApiClient(HttpClient httpClient) : IFinanceControlApiClient
{
    private readonly HttpClient _httpClient = httpClient;

    public Task<HttpResponseMessage> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default) =>
        _httpClient.PostAsJsonAsync("user/login", request, cancellationToken);

    public Task<HttpResponseMessage> RegisterAsync(RegisterUserDto request, CancellationToken cancellationToken = default) =>
        _httpClient.PostAsJsonAsync("user/register", request, cancellationToken);

    public Task<HttpResponseMessage> RegisterCategoryAsync(CategoryRegisterDto request, CancellationToken cancellationToken = default) =>
        _httpClient.PostAsJsonAsync("api/Category/registerCategory", request, cancellationToken);

    public Task<HttpResponseMessage> GetCategoriesAsync(CancellationToken cancellationToken = default) =>
        _httpClient.GetAsync("api/Category", cancellationToken);

    public Task<HttpResponseMessage> GetTransactionsAsync(CancellationToken cancellationToken = default) =>
        _httpClient.GetAsync("api/Transaction", cancellationToken);

    public Task<HttpResponseMessage> CreateTransactionAsync(TransactionCreateDto request, CancellationToken cancellationToken = default) =>
        _httpClient.PostAsJsonAsync("api/Transaction", request, cancellationToken);

    public Task<HttpResponseMessage> GetTransactionTypesAsync(CancellationToken cancellationToken = default) =>
        _httpClient.GetAsync("api/TransactionTypes", cancellationToken);

    public Task<HttpResponseMessage> GetPaymentMethodsAsync(CancellationToken cancellationToken = default) =>
        _httpClient.GetAsync("api/PaymentMethods", cancellationToken);

    public Task<HttpResponseMessage> RegisterPaymentMethodAsync(PaymentMethodRegisterDto request, CancellationToken cancellationToken = default) =>
        _httpClient.PostAsJsonAsync("api/PaymentMethods/registerPaymentMethod", request, cancellationToken);

    public Task<HttpResponseMessage> DeletePaymentMethodAsync(int paymentMethodId, CancellationToken cancellationToken = default) =>
        _httpClient.DeleteAsync($"api/PaymentMethods/{paymentMethodId}", cancellationToken);

    public Task<HttpResponseMessage> DeleteCategoryAsync(int categoryId, CancellationToken cancellationToken = default) =>
        _httpClient.DeleteAsync($"api/Category/{categoryId}", cancellationToken);

    public Task<HttpResponseMessage> DeleteTransactionAsync(int transactionId, CancellationToken cancellationToken = default) =>
        _httpClient.DeleteAsync($"api/Transaction/{transactionId}", cancellationToken);

    public Task<HttpResponseMessage> GetAccountByIdAsync(int accountId, CancellationToken cancellationToken = default) =>
        _httpClient.GetAsync($"api/Account/{accountId}", cancellationToken);
}
