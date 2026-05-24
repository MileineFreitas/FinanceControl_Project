using FinanceControl.Client.Services.Interfaces.TransactionTypes;
using FinanceControl.Contracts.Constants;
using FinanceControl.Contracts.Dtos.TransactionTypes;
using FinanceControl.Web.Models.ViewModels.TransactionTypes;
using Microsoft.AspNetCore.Mvc;

namespace FinanceControl.Web.Controllers.TransactionTypes;

/// <summary>Meios de pagamento (débito, crédito, dinheiro, PIX, etc.). Receita/despesa fica na transação.</summary>
[Route("tipos-transacao")]
public class TransactionTypesController(ITransactionTypeCliService transactionTypeCli) : Controller
{
    [HttpGet("")]
    [HttpGet("Index")]
    public async Task<IActionResult> Index(string? busca)
    {
        var vm = new TransactionTypeViewModel { Busca = busca };
        await LoadListAsync(vm);
        return View(vm);
    }

    [HttpPost("Save")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(TransactionTypeViewModel vm)
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
        if (vm.EditingId is int id && id > 0)
        {
            var existing = await transactionTypeCli.GetByIdAsync(id);
            if (existing == null)
            {
                vm.ErroModal = "Meio de pagamento não encontrado.";
                return View("Index", vm);
            }

            var update = new TransactionTypeUpdateDto
            {
                TransactionTypeId = id,
                Name = vm.Input.Name,
                Code = existing.IsSystem ? existing.Code : PaymentMethodCodes.FromName(vm.Input.Name),
                Icon = vm.Input.Icon,
                PaymentKind = existing.PaymentKind,
                Description = vm.Input.Description?.Trim(),
                IsActive = existing.IsActive
            };
            response = await transactionTypeCli.UpdateAsync(id, update);
        }
        else
        {
            vm.Input.Code = PaymentMethodCodes.FromName(vm.Input.Name);
            vm.Input.PaymentKind = null;
            vm.Input.IsActive = true;
            response = await transactionTypeCli.CreateAsync(vm.Input);
        }

        if (!response.IsSuccessStatusCode)
        {
            vm.ErroModal = await ReadErrorAsync(response);
            return View("Index", vm);
        }

        return RedirectToAction(nameof(Index), new { busca = vm.Busca });
    }

    [HttpGet("Edit/{id:int}")]
    public async Task<IActionResult> Edit(int id, string? busca)
    {
        var vm = new TransactionTypeViewModel { EditingId = id, ModalAberto = true, Busca = busca };
        var dto = await transactionTypeCli.GetByIdAsync(id);
        if (dto != null)
        {
            vm.Input = new TransactionTypeCreateDto
            {
                Name = dto.Name,
                Icon = PaymentMethodIcons.Normalize(dto.Icon),
                Description = dto.Description
            };
        }

        await LoadListAsync(vm);
        return View("Index", vm);
    }

    [HttpPost("Delete/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, string? busca)
    {
        var vm = new TransactionTypeViewModel { Busca = busca };
        var res = await transactionTypeCli.DeleteAsync(id);
        if (!res.IsSuccessStatusCode)
        {
            vm.ErroPagina = res.StatusCode == System.Net.HttpStatusCode.Conflict
                ? "Não é possível excluir este meio de pagamento."
                : await ReadErrorAsync(res);
        }

        await LoadListAsync(vm);
        return View("Index", vm);
    }

    private async Task LoadListAsync(TransactionTypeViewModel vm)
    {
        try
        {
            var list = await transactionTypeCli.ListAsync(includeInactive: true);
            if (list is { Count: > 0 })
            {
                vm.UsandoDadosDemo = false;
                vm.Tipos = list.Select(TransactionTypeViewModelMapper.ToItem).ToList();
                ApplyBusca(vm);
                return;
            }
        }
        catch
        {
            /* demo fallback */
        }

        vm.UsandoDadosDemo = true;
        vm.Tipos = GetDemoFallback();
        ApplyBusca(vm);
    }

    private static void ApplyBusca(TransactionTypeViewModel vm)
    {
        if (string.IsNullOrWhiteSpace(vm.Busca)) return;
        var term = vm.Busca.Trim();
        vm.Tipos = vm.Tipos
            .Where(t => t.Nome.Contains(term, StringComparison.OrdinalIgnoreCase)
                        || t.Codigo.Contains(term, StringComparison.OrdinalIgnoreCase)
                        || t.MeioPagamento.Contains(term, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    private static List<TransactionTypeItemVm> GetDemoFallback() =>
    [
        new(1, "💳", "Débito", "DEBITO", "Débito", "Ativo", true),
        new(2, "💳", "Crédito", "CREDITO", "Crédito", "Ativo", true),
        new(3, "💵", "Dinheiro", "DINHEIRO", "Dinheiro", "Ativo", true),
        new(4, "📱", "PIX", "PIX", "Personalizado", "Ativo", false),
    ];

    private static async Task<string> ReadErrorAsync(HttpResponseMessage response)
    {
        var body = await response.Content.ReadAsStringAsync();
        return string.IsNullOrWhiteSpace(body)
            ? $"Erro HTTP {(int)response.StatusCode}"
            : body;
    }
}
