using FinanceControl.Contracts.Dtos.Auth;

namespace FinanceControl.Web.Models.ViewModels.Auth;

public sealed class LoginViewModel
{
    public LoginRequestDto LoginRequest { get; set; } = new();

    public string? Message { get; set; }
}
