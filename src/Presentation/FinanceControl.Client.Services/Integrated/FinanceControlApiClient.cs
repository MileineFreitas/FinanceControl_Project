using System.Net.Http.Json;
using FinanceControl.Client.Services.Interfaces;
using FinanceControl.Contracts.Dtos.Auth;
using FinanceControl.Contracts.Dtos.Categories;
using FinanceControl.Contracts.Dtos.Transactions;
using FinanceControl.Contracts.Dtos.Users;

namespace FinanceControl.Client.Services.Integrated;

public sealed class FinanceControlApiClient(HttpClient httpClient) : IFinanceControlApiClient
{
    public Task<HttpResponseMessage> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default) =>
        httpClient.PostAsJsonAsync("User/login", request, cancellationToken);

    public Task<HttpResponseMessage> RegisterAsync(RegisterUserDto request, CancellationToken cancellationToken = default) =>
        httpClient.PostAsJsonAsync("User/register", request, cancellationToken);

    public Task<HttpResponseMessage> RegisterCategoryAsync(CategoryRegisterDto request, CancellationToken cancellationToken = default) =>
        httpClient.PostAsJsonAsync("api/Category/registerCategory", request, cancellationToken);

    public Task<HttpResponseMessage> GetCategoriesAsync(CancellationToken cancellationToken = default) =>
        httpClient.GetAsync("api/Category", cancellationToken);

    public Task<HttpResponseMessage> GetTransactionsAsync(CancellationToken cancellationToken = default) =>
        httpClient.GetAsync("api/Transaction", cancellationToken);

    public Task<HttpResponseMessage> CreateTransactionAsync(TransactionCreateDto request, CancellationToken cancellationToken = default) =>
        httpClient.PostAsJsonAsync("api/Transaction", request, cancellationToken);

    public Task<HttpResponseMessage> GetTransactionTypesAsync(CancellationToken cancellationToken = default) =>
        httpClient.GetAsync("api/TransactionTypes", cancellationToken);

    public Task<HttpResponseMessage> DeleteCategoryAsync(int categoryId, CancellationToken cancellationToken = default) =>
        httpClient.DeleteAsync($"api/Category/{categoryId}", cancellationToken);

    public Task<HttpResponseMessage> DeleteTransactionAsync(int transactionId, CancellationToken cancellationToken = default) =>
        httpClient.DeleteAsync($"api/Transaction/{transactionId}", cancellationToken);

    public Task<HttpResponseMessage> GetAccountByIdAsync(int accountId, CancellationToken cancellationToken = default) =>
        httpClient.GetAsync($"api/Account/{accountId}", cancellationToken);
}
