using FinanceControl.Client.Services.Interfaces.PaymentMethods;
using FinanceControl.Contracts.Constants;
using FinanceControl.Contracts.Dtos.PaymentMethods;
using FinanceControl.Web.Helpers;
using FinanceControl.Web.Models.ViewModels.PaymentMethods;
using Microsoft.AspNetCore.Mvc;

namespace FinanceControl.Web.Controllers.PaymentMethods;

[Route("meios-pagamento")]
public class PaymentMethodsController(IPaymentMethodCliService paymentMethodCli) : Controller
{
    [HttpGet("")]
    [HttpGet("Index")]
    public async Task<IActionResult> Index(string? busca)
    {
        var vm = new PaymentMethodViewModel { Busca = busca };
        await LoadListAsync(vm);
        return View(vm);
    }

    [HttpGet("~/tipos-transacao")]
    [HttpGet("~/tipos-transacao/{*path}")]
    public IActionResult LegacyRedirect() =>
        RedirectToActionPermanent(nameof(Index));

    [HttpPost("Save")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(PaymentMethodViewModel vm)
    {
        vm.ModalAberto = true;
        await LoadListAsync(vm);

        if (string.IsNullOrWhiteSpace(vm.Input.Name) || vm.Input.Name.Trim().Length < 2)
        {
            vm.ErroModal = "Nome deve ter pelo menos 2 caracteres.";
            return View("Index", vm);
        }

        vm.Input.Icon = PaymentMethodIcons.Normalize(vm.Input.Icon);
        vm.Input.Name = vm.Input.Name.Trim();

        HttpResponseMessage response;
        if (vm.EditingId is Guid id && id != Guid.Empty)
        {
            var existing = await paymentMethodCli.GetByIdAsync(id);
            if (existing == null)
            {
                vm.ErroModal = "Meio de pagamento não encontrado.";
                return View("Index", vm);
            }

            var update = new PaymentMethodUpdateDto
            {
                PaymentMethodId = id,
                Name = vm.Input.Name,
                Icon = vm.Input.Icon,
                Description = vm.Input.Description?.Trim(),
                IsActive = existing.IsActive
            };
            response = await paymentMethodCli.UpdateAsync(id, update);
        }
        else
        {
            vm.Input.IsActive = true;
            vm.Input.UserId = User.GetUserId();
            response = await paymentMethodCli.CreateAsync(vm.Input);
        }

        if (!response.IsSuccessStatusCode)
        {
            vm.ErroModal = await ReadErrorAsync(response);
            return View("Index", vm);
        }

        return RedirectToAction(nameof(Index), new { busca = vm.Busca });
    }

    [HttpGet("Edit/{id:guid}")]
    public async Task<IActionResult> Edit(Guid id, string? busca)
    {
        var vm = new PaymentMethodViewModel { EditingId = id, ModalAberto = true, Busca = busca };
        var dto = await paymentMethodCli.GetByIdAsync(id);
        if (dto != null)
        {
            vm.Input = new PaymentMethodCreateDto
            {
                Name = dto.Name,
                Icon = PaymentMethodIcons.Normalize(dto.Icon),
                Description = dto.Description
            };
        }

        await LoadListAsync(vm);
        return View("Index", vm);
    }

    [HttpPost("Delete/{id:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, string? busca)
    {
        var vm = new PaymentMethodViewModel { Busca = busca };
        var res = await paymentMethodCli.DeleteAsync(id);
        if (!res.IsSuccessStatusCode)
        {
            vm.ErroPagina = res.StatusCode == System.Net.HttpStatusCode.Conflict
                ? "Não é possível excluir este meio de pagamento."
                : await ReadErrorAsync(res);
        }

        await LoadListAsync(vm);
        return View("Index", vm);
    }

    private async Task LoadListAsync(PaymentMethodViewModel vm)
    {
        try
        {
            var list = await paymentMethodCli.ListAsync(includeInactive: true, userId: User.GetUserId());
            vm.Meios = (list ?? []).Select(PaymentMethodViewModelMapper.ToItem).ToList();
            ApplyBusca(vm);
        }
        catch (Exception ex)
        {
            vm.ErroPagina = $"Não foi possível carregar meios de pagamento: {ex.Message}";
            vm.Meios = [];
        }
    }

    private static void ApplyBusca(PaymentMethodViewModel vm)
    {
        if (string.IsNullOrWhiteSpace(vm.Busca)) return;
        var term = vm.Busca.Trim();
        vm.Meios = vm.Meios
            .Where(t => t.Nome.Contains(term, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    private static async Task<string> ReadErrorAsync(HttpResponseMessage response)
    {
        var body = await response.Content.ReadAsStringAsync();
        return string.IsNullOrWhiteSpace(body)
            ? $"Erro HTTP {(int)response.StatusCode}"
            : body;
    }
}
