using System.Net.Http.Json;
using FinanceControl.Client.Services.Interfaces;
using FinanceControl.Contracts.Dtos.Auth;
using FinanceControl.Contracts.Dtos.Categories;
using FinanceControl.Contracts.Dtos.Transactions;
using FinanceControl.Contracts.Dtos.Users;

namespace FinanceControl.Client.Services.Integrated;

public sealed class FinanceControlApiClient(HttpClient httpClient) : IFinanceControlApiClient
{
    public Task<HttpResponseMessage> LoginAsync(LoginRequestDto request) =>
        httpClient.PostAsJsonAsync("User/login", request);

    public Task<HttpResponseMessage> RegisterAsync(RegisterUserDto request) =>
        httpClient.PostAsJsonAsync("User/register", request);

    public Task<HttpResponseMessage> RegisterCategoryAsync(CategoryRegisterDto request) =>
        httpClient.PostAsJsonAsync("api/Category/registerCategory", request);

    public Task<HttpResponseMessage> GetCategoriesAsync() =>
        httpClient.GetAsync("api/Category");

    public Task<HttpResponseMessage> GetTransactionsAsync() =>
        httpClient.GetAsync("api/Transaction");

    public Task<HttpResponseMessage> CreateTransactionAsync(TransactionCreateDto request) =>
        httpClient.PostAsJsonAsync("api/Transaction", request);

    public Task<HttpResponseMessage> GetPaymentMethodsAsync() =>
        httpClient.GetAsync("api/PaymentMethods");

    public Task<HttpResponseMessage> DeleteCategoryAsync(Guid categoryId) =>
        httpClient.DeleteAsync($"api/Category/{categoryId}");

    public Task<HttpResponseMessage> DeleteTransactionAsync(Guid transactionId) =>
        httpClient.DeleteAsync($"api/Transaction/{transactionId}");

    public Task<HttpResponseMessage> GetAccountByIdAsync(Guid accountId) =>
        httpClient.GetAsync($"api/Account/{accountId}");
}
