using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FinanceControl.Contracts.Dtos.Auth;
using FinanceControl.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FinanceControl.Web.Pages;

public class IndexModel : PageModel
{
    private readonly IFinanceControlApiClient _api;

    public IndexModel(IFinanceControlApiClient api)
    {
        _api = api;
    }

    [BindProperty]
    public LoginRequestDto LoginRequest { get; set; } = new();

    public string? Message { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(LoginRequest.Email) || string.IsNullOrWhiteSpace(LoginRequest.Password))
        {
            Message = "Informe e-mail e senha.";
            return Page();
        }

        Message = "Enviando dados para API...";

        try
        {
            var response = await _api.LoginAsync(LoginRequest, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>(
                    cancellationToken: cancellationToken);

                if (result != null)
                    Message = $"Bem-vindo, {result.Name}!";

                return RedirectToPage("/Home");
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                Message = "Credenciais inválidas. Por favor, tente novamente.";
                return Page();
            }

            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            Message = $"Status: {response.StatusCode} — {body}";
        }
        catch (Exception ex)
        {
            Message = $"Erro: {ex.Message}";
        }

        return Page();
    }
}
