using System.Net;
using System.Text.Json;
using FinanceControl.Client.Services.Interfaces.Users;
using FinanceControl.Contracts.Dtos.Users;
using FinanceControl.Web.Helpers;
using FinanceControl.Web.Models.ViewModels.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace FinanceControl.Web.Controllers.Account;

[Route("conta")]
public class AccountController(IUserCliService userCli) : Controller
{
    [HttpGet("configuracao")]
    public async Task<IActionResult> Configuracao()
    {
        var vm = new ConfigurationViewModel();
        var userId = User.GetUserId();
        if (userId == null)
            return RedirectToAction("Index", "Login");

        try
        {
            var user = await userCli.GetByIdAsync(userId.Value);
            if (user != null)
            {
                vm.Moeda = user.Moeda;
                vm.Idioma = user.Idioma;
                vm.FormatoData = user.FormatoData;
                vm.InicioMes = user.InicioMes;
            }
        }
        catch
        {
            var prefs = User.GetFinancialPreferences();
            vm.Moeda = prefs.Moeda;
            vm.Idioma = prefs.Idioma;
            vm.FormatoData = prefs.FormatoData;
            vm.InicioMes = prefs.InicioMes;
        }

        return View("Configuration", vm);
    }

    [HttpPost("configuracao")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Configuracao(ConfigurationViewModel vm)
    {
        var userId = User.GetUserId();
        if (userId == null)
            return RedirectToAction("Index", "Login");

        var dto = new UserFinancialPreferencesDto
        {
            Moeda = vm.Moeda,
            Idioma = vm.Idioma,
            FormatoData = vm.FormatoData,
            InicioMes = vm.InicioMes
        };

        try
        {
            var response = await userCli.UpdateFinancialPreferencesAsync(userId.Value, dto);
            if (!response.IsSuccessStatusCode)
            {
                TempData["ConfigErro"] = "Não foi possível salvar as preferências financeiras. Tente novamente.";
                return RedirectToAction(nameof(Configuracao));
            }

            await HttpContext.RefreshFinancialPreferencesAsync(dto);
            TempData["ConfigSucesso"] = "Preferências financeiras salvas com sucesso.";
        }
        catch
        {
            TempData["ConfigErro"] = "Não foi possível salvar as preferências financeiras. Tente novamente.";
        }

        return RedirectToAction(nameof(Configuracao));
    }

    [HttpPost("encerrar-sessoes")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EncerrarSessoes()
    {
        var userId = User.GetUserId();
        if (userId == null)
            return RedirectToAction("Index", "Login");

        try
        {
            var response = await userCli.RevokeOtherSessionsAsync(userId.Value);
            if (!response.IsSuccessStatusCode)
            {
                TempData["ConfigErro"] = "Não foi possível encerrar as sessões. Tente novamente.";
                return RedirectToAction(nameof(Configuracao));
            }

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["LoginInfo"] = "Todas as sessões foram encerradas. Faça login novamente.";
            return RedirectToAction("Index", "Login");
        }
        catch (Exception)
        {
            TempData["ConfigErro"] = "Não foi possível encerrar as sessões. Tente novamente.";
        }

        return RedirectToAction(nameof(Configuracao));
    }

    [HttpPost("excluir-conta")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExcluirConta([FromForm] string? SenhaExclusao)
    {
        var userId = User.GetUserId();
        if (userId == null)
            return RedirectToAction("Index", "Login");

        if (string.IsNullOrWhiteSpace(SenhaExclusao))
        {
            TempData["ConfigErro"] = "Informe sua senha para confirmar a exclusão da conta.";
            return RedirectToAction(nameof(Configuracao));
        }

        try
        {
            var response = await userCli.DeleteAccountAsync(userId.Value, SenhaExclusao);
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                TempData["ConfigErro"] = response.StatusCode == HttpStatusCode.BadRequest
                    ? ExtrairMensagemErro(body)
                    : "Não foi possível excluir a conta. Tente novamente.";
                return RedirectToAction(nameof(Configuracao));
            }

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["LoginInfo"] = "Sua conta foi excluída permanentemente.";
            return RedirectToAction("Index", "Login");
        }
        catch (Exception)
        {
            TempData["ConfigErro"] = "Não foi possível excluir a conta. Tente novamente.";
        }

        return RedirectToAction(nameof(Configuracao));
    }

    private static string ExtrairMensagemErro(string body)
    {
        try
        {
            using var json = JsonDocument.Parse(body);
            if (json.RootElement.ValueKind == JsonValueKind.Object &&
                json.RootElement.TryGetProperty("message", out var message) &&
                message.ValueKind == JsonValueKind.String)
            {
                return message.GetString() ?? body;
            }
        }
        catch (JsonException)
        {
        }

        return body;
    }

    [HttpGet("privacidade")]
    public IActionResult Privacidade() => View("Privacy");
}
