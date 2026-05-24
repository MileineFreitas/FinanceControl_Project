using System.Net;
using System.Net.Http.Json;
using FinanceControl.Client.Services.Interfaces;
using FinanceControl.Contracts.Dtos.Auth;
using FinanceControl.Web.Models.ViewModels.Auth;
using Microsoft.AspNetCore.Mvc;

namespace FinanceControl.Web.Controllers.Auth;

[Route("")]
public class LoginController : Controller
{
    private readonly IFinanceControlApiClient _api;

    public LoginController(IFinanceControlApiClient api) => _api = api;

    [HttpGet("")]
    [HttpGet("login")]
    public IActionResult Index() => View(new LoginViewModel());

    [HttpPost("")]
    [HttpPost("login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(LoginViewModel vm)
    {
        if (string.IsNullOrWhiteSpace(vm.LoginRequest.Email) || string.IsNullOrWhiteSpace(vm.LoginRequest.Password))
        {
            vm.Message = "Informe e-mail e senha.";
            return View(vm);
        }

        vm.Message = "Enviando dados para API...";

        try
        {
            var response = await _api.LoginAsync(vm.LoginRequest);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>(
                    );

                if (result != null)
                    vm.Message = $"Bem-vindo, {result.Name}!";

                return RedirectToAction(nameof(Index), "Home");
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                vm.Message = "Credenciais inválidas. Por favor, tente novamente.";
                return View(vm);
            }

            var body = await response.Content.ReadAsStringAsync();
            vm.Message = $"Status: {response.StatusCode} — {body}";
        }
        catch (Exception ex)
        {
            vm.Message = $"Erro: {ex.Message}";
        }

        return View("Login", vm);
    }
}
