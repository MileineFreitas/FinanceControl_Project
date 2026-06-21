using FinanceControl.Contracts.Dtos.Auth;
using FinanceControl.Contracts.Dtos.Users;

namespace FinanceControl.Client.Services.Interfaces;

/// <summary>Cliente HTTP para fluxos de autenticação (login e registo).</summary>
public interface IFinanceControlApiClient
{
    Task<HttpResponseMessage> LoginAsync(LoginRequestDto request);

    Task<HttpResponseMessage> RegisterAsync(RegisterUserDto request);
}
