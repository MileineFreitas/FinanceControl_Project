using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using FinanceControl.Client.Services.Interfaces;
using FinanceControl.Contracts.Dtos.Auth;
using FinanceControl.Web.Models.ViewModels.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceControl.Web.Controllers.Auth;

[AllowAnonymous]
[Route("")]
public class LoginController : Controller
{
    private readonly IFinanceControlApiClient _api;

    public LoginController(IFinanceControlApiClient api) => _api = api;

    [HttpGet("")]
    [HttpGet("login")]
    public IActionResult Index()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");

        return View(new LoginViewModel());
    }

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

        try
        {
            var response = await _api.LoginAsync(vm.LoginRequest);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
                if (result == null)
                {
                    vm.Message = "Resposta inválida da API.";
                    return View(vm);
                }

                await SignInUserAsync(result);
                return RedirectToAction("Index", "Home");
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

        return View(vm);
    }

    [HttpPost("logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Index));
    }

    private async Task SignInUserAsync(LoginResponseDto user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Email, user.Email),
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8),
            });
    }
}
