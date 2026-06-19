using FinanceControl.Client.Services.Interfaces.Categories;
using FinanceControl.Contracts.Constants;
using FinanceControl.Contracts.Dtos.Categories;
using FinanceControl.Contracts.Filters;
using FinanceControl.Web.Models.ViewModels.Categories;
using Microsoft.AspNetCore.Mvc;

namespace FinanceControl.Web.Controllers.Categories;

[Route("categorias")]
public class CategoriesController(ICategoryCliService categoryCli) : Controller
{
    [HttpGet("")]
    [HttpGet("Index")]
    public async Task<IActionResult> Index(string? busca)
    {
        var vm = new CategoryViewModel { Busca = busca };
        await LoadListAsync(vm);
        return View(vm);
    }

    [HttpPost("Save")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(CategoryViewModel vm)
    {
        vm.ModalAberto = true;
        await LoadListAsync(vm);

        if (string.IsNullOrWhiteSpace(vm.Input.CategoryName) || vm.Input.CategoryName.Length < 2)
        {
            vm.ErroModal = "Nome deve ter pelo menos 2 caracteres.";
            return View("Index", vm);
        }

        vm.Input.Icon = CategoryIcons.Normalize(vm.Input.Icon);

        HttpResponseMessage response;
        if (vm.EditingId is Guid id && id != Guid.Empty)
        {
            var update = new CategoryUpdateDto
            {
                CategoryId = id,
                CategoryName = vm.Input.CategoryName.Trim(),
                Description = vm.Input.CategoryDescription?.Trim(),
                Icon = vm.Input.Icon
            };
            response = await categoryCli.UpdateAsync(id, update);
        }
        else
        {
            response = await categoryCli.CreateAsync(vm.Input);
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
        var vm = new CategoryViewModel { EditingId = id, ModalAberto = true, Busca = busca };
        var dto = await categoryCli.GetByIdAsync(id);
        if (dto != null)
        {
            vm.Input = new CategoryRegisterDto
            {
                CategoryName = dto.CategoryName ?? "",
                CategoryDescription = dto.Description ?? "",
                Icon = CategoryIcons.Normalize(dto.Icon)
            };
        }

        await LoadListAsync(vm);
        return View("Index", vm);
    }

    [HttpPost("Delete/{id:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, string? busca)
    {
        var vm = new CategoryViewModel { Busca = busca };
        var res = await categoryCli.DeleteAsync(id);
        if (!res.IsSuccessStatusCode)
        {
            vm.ErroPagina = res.StatusCode == System.Net.HttpStatusCode.Conflict
                ? "Não é possível excluir: existem transações vinculadas."
                : await ReadErrorAsync(res);
        }

        await LoadListAsync(vm);
        return View("Index", vm);
    }

    private async Task LoadListAsync(CategoryViewModel vm)
    {
        var filter = new DataFilterDto { Page = 1, PageSize = 200 };
        if (!string.IsNullOrWhiteSpace(vm.Busca))
            filter.Filters = new Dictionary<string, string> { ["search"] = vm.Busca.Trim() };

        try
        {
            var data = await categoryCli.ListAsync(filter);
            vm.Categorias = (data?.Result ?? []).Select(CategoryViewModelMapper.ToItem).ToList();
        }
        catch (Exception ex)
        {
            vm.ErroPagina = $"Não foi possível carregar categorias: {ex.Message}";
            vm.Categorias = [];
        }
    }

    private static async Task<string> ReadErrorAsync(HttpResponseMessage response)
    {
        var body = await response.Content.ReadAsStringAsync();
        return string.IsNullOrWhiteSpace(body)
            ? $"Erro HTTP {(int)response.StatusCode}"
            : body;
    }
}
