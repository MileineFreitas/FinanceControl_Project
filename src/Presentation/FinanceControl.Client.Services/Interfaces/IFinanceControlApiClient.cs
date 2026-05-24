using FinanceControl.Contracts.Dtos.Auth;
using FinanceControl.Contracts.Dtos.Categories;
using FinanceControl.Contracts.Dtos.Transactions;
using FinanceControl.Contracts.Dtos.Users;

namespace FinanceControl.Client.Services.Interfaces;

/// <summary>Cliente HTTP agregado (legado). Preferir I{Entidade}CliService por feature.</summary>
public interface IFinanceControlApiClient
{
    Task<HttpResponseMessage> LoginAsync(LoginRequestDto request);

    Task<HttpResponseMessage> RegisterAsync(RegisterUserDto request);

    Task<HttpResponseMessage> RegisterCategoryAsync(CategoryRegisterDto request);

    Task<HttpResponseMessage> GetCategoriesAsync();

    Task<HttpResponseMessage> GetTransactionsAsync();

    Task<HttpResponseMessage> CreateTransactionAsync(TransactionCreateDto request);

    Task<HttpResponseMessage> GetTransactionTypesAsync();

    Task<HttpResponseMessage> DeleteCategoryAsync(int categoryId);

    Task<HttpResponseMessage> DeleteTransactionAsync(int transactionId);

    Task<HttpResponseMessage> GetAccountByIdAsync(int accountId);
}
