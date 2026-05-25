using FinanceControl.Client.Services.Interfaces.Users;
using FinanceControl.Contracts.Dtos.Users;
using FinanceControl.Contracts.Filters;
using FinanceControl.Web.Models.ViewModels.Profile;
using Microsoft.AspNetCore.Mvc;

namespace FinanceControl.Web.Controllers.Profile;

[Route("perfil")]
public class ProfileController(IUserCliService userCli) : Controller
{
    [HttpGet("")]
    [HttpGet("Index")]
    public async Task<IActionResult> Index()
    {
        var vm = new ProfileViewModel();
        await CarregarUsuarioAsync(vm);
        return View("Index", vm);
    }

    [HttpPost("Salvar")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Salvar(ProfileViewModel vm)
    {
        if (vm.Input.UserId == Guid.Empty)
        {
            vm.ErroPagina = "Nenhum usuário disponível para atualizar.";
            return View("Index", vm);
        }

        if (string.IsNullOrWhiteSpace(vm.Input.UserName) || vm.Input.UserName.Trim().Length < 3)
        {
            vm.ErroPagina = "Informe um nome de usuário com pelo menos 3 caracteres.";
            await CarregarUsuarioAsync(vm);
            return View("Index", vm);
        }

        if (string.IsNullOrWhiteSpace(vm.Input.Email))
        {
            vm.ErroPagina = "Informe um e-mail válido.";
            await CarregarUsuarioAsync(vm);
            return View("Index", vm);
        }

        vm.Input.UserName = vm.Input.UserName.Trim();
        vm.Input.Email = vm.Input.Email.Trim();

        if (vm.ProfilePhotoFile is { Length: > 0 })
        {
            await using var ms = new MemoryStream();
            await vm.ProfilePhotoFile.CopyToAsync(ms);
            vm.Input.ProfilePhoto = Convert.ToBase64String(ms.ToArray());
        }

        if (string.IsNullOrWhiteSpace(vm.Input.Password))
            vm.Input.Password = null;

        try
        {
            var res = await userCli.UpdateAsync(vm.Input.UserId, vm.Input);
            if (!res.IsSuccessStatusCode)
            {
                var body = await res.Content.ReadAsStringAsync();
                vm.ErroPagina = string.IsNullOrWhiteSpace(body)
                    ? $"Não foi possível salvar ({(int)res.StatusCode})."
                    : body;
                await CarregarUsuarioAsync(vm);
                return View("Index", vm);
            }

            TempData["ProfileSucesso"] = "Perfil atualizado com sucesso.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            vm.ErroPagina = $"Não foi possível salvar o perfil: {ex.Message}";
            await CarregarUsuarioAsync(vm);
            return View("Index", vm);
        }
    }

    private async Task CarregarUsuarioAsync(ProfileViewModel vm)
    {
        vm.TemUsuario = false;
        try
        {
            var data = await userCli.ListAsync(new DataFilterDto { Page = 1, PageSize = 50 });
            var user = data?.Result?.OrderBy(u => u.UserId).FirstOrDefault();
            if (user == null)
                return;

            vm.TemUsuario = true;
            vm.Input = new UserUpdateDto
            {
                UserId = user.UserId,
                UserName = user.UserName ?? "",
                Email = user.UserEmail ?? "",
                IsActive = user.IsActive,
                ProfilePhoto = user.ProfilePhoto
            };
        }
        catch (Exception ex)
        {
            vm.ErroPagina ??= $"Não foi possível carregar o perfil: {ex.Message}";
        }
    }
}
