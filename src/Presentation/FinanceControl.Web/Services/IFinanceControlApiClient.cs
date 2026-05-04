using FinanceControl.Contracts.Dtos.Auth;
using FinanceControl.Domain.Entities;

namespace FinanceControl.Web.Services;

/// <summary>Cliente HTTP tipado para a API FinanceControl.</summary>
public interface IFinanceControlApiClient
{
    Task<HttpResponseMessage> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);

    Task<HttpResponseMessage> RegisterAsync(RegisterUserDto request, CancellationToken cancellationToken = default);

    Task<HttpResponseMessage> RegisterCategoryAsync(CategoryRegisterDto request, CancellationToken cancellationToken = default);

    Task<HttpResponseMessage> GetCategoriesAsync(CancellationToken cancellationToken = default);

    Task<HttpResponseMessage> GetTransactionsAsync(CancellationToken cancellationToken = default);

    Task<HttpResponseMessage> CreateTransactionAsync(TransactionCreateDto request, CancellationToken cancellationToken = default);

    Task<HttpResponseMessage> GetTransactionTypesAsync(CancellationToken cancellationToken = default);

    Task<HttpResponseMessage> DeleteCategoryAsync(int categoryId, CancellationToken cancellationToken = default);

    Task<HttpResponseMessage> DeleteTransactionAsync(int transactionId, CancellationToken cancellationToken = default);

    Task<HttpResponseMessage> GetAccountByIdAsync(int accountId, CancellationToken cancellationToken = default);
}
