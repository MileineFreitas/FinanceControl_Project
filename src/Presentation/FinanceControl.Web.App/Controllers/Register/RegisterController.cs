using FinanceControl.Client.Services.Interfaces;
using FinanceControl.Web.Models.ViewModels.Register;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceControl.Web.Controllers.Register;

[AllowAnonymous]
[Route("register")]
public class RegisterController : Controller
{
    private readonly IFinanceControlApiClient _api;

    public RegisterController(IFinanceControlApiClient api) => _api = api;

    [HttpGet("")]
    [HttpGet("Index")]
    public IActionResult Index() => View("Index", new RegisterViewModel());

    [HttpPost("")]
    [HttpPost("Index")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(RegisterViewModel vm)
    {
        if (!TryValidateModel(vm.Register))
            return View("Index", vm);

        if (vm.ProfilePhotoFile is { Length: > 0 })
        {
            await using var ms = new MemoryStream();
            await vm.ProfilePhotoFile.CopyToAsync(ms);
            var contentType = string.IsNullOrWhiteSpace(vm.ProfilePhotoFile.ContentType)
                ? "image/jpeg"
                : vm.ProfilePhotoFile.ContentType;
            vm.Register.ProfilePhoto = $"data:{contentType};base64,{Convert.ToBase64String(ms.ToArray())}";
        }

        try
        {
            var response = await _api.RegisterAsync(vm.Register);
            if (!response.IsSuccessStatusCode)
            {
                vm.ErrorMessage = $"Erro ao criar conta: {await response.Content.ReadAsStringAsync()}";
                return View("Index", vm);
            }

            return RedirectToAction(nameof(Index), "Login");
        }
        catch (Exception ex)
        {
            vm.ErrorMessage = ex.Message;
            return View("Index", vm);
        }
    }
}
