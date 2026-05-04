using System.Net;
using System.Text.Json;
using FinanceControl.Domain.Entities;
using FinanceControl.Domain.Enums;
using FinanceControl.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FinanceControl.Web.Pages.Categories;

public class IndexModel : PageModel
{
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };
    private static readonly object SeedLock = new();
    private static List<CategoriaVm>? _demoFallback;

    private readonly IFinanceControlApiClient _api;

    public IndexModel(IFinanceControlApiClient api) => _api = api;

    [BindProperty(SupportsGet = true)]
    public string? Abrir { get; set; }

    [BindProperty]
    public CategoryRegisterDto CategoryInput { get; set; } = new();

    [BindProperty]
    public string SelectedIcon { get; set; } = "💰";

    public bool ModalAberto { get; set; }

    public string? ErroModal { get; set; }

    public string? ErroPagina { get; set; }

    public bool UsandoDadosDemo { get; set; }

    public List<CategoriaVm> Categorias { get; private set; } = [];

    public IReadOnlyList<string> IconesDisponiveis { get; } =
    [
        "🍔", "🚗", "🏠", "💊", "📚", "🎮", "✈️", "👕", "💡", "🐶",
        "💰", "🎵", "🏋️", "🎁", "💼", "📈", "🧳", "🛒",
    ];

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        if (string.Equals(Abrir?.Trim(), "novo", StringComparison.OrdinalIgnoreCase))
            ModalAberto = true;

        await CarregarCategoriasAsync(cancellationToken);
    }

    public async Task<IActionResult> OnPostCreateCategoryAsync(CancellationToken cancellationToken)
    {
        ModalAberto = true;
        await CarregarCategoriasAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(CategoryInput.CategoryName) || CategoryInput.CategoryName.Length < 2)
        {
            ErroModal = "Nome deve ter pelo menos 2 caracteres.";
            return Page();
        }

        // API ainda associa um TransactionTypeId; na UI nao escolhemos — usa despesa como valor tecnico padrao.
        CategoryInput.Type = TransactionType.Despesa;

        try
        {
            var response = await _api.RegisterCategoryAsync(CategoryInput, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                ErroModal = $"Erro ao salvar: {await response.Content.ReadAsStringAsync(cancellationToken)}";
                return Page();
            }

            ModalAberto = false;
            ErroModal = null;
            CategoryInput = new CategoryRegisterDto();
            SelectedIcon = "💰";
            return RedirectToPage("/categorias");
        }
        catch (Exception ex)
        {
            ErroModal = $"Erro inesperado: {ex.Message}";
            return Page();
        }
    }

    public async Task<IActionResult> OnPostDeleteCategoryAsync(int id, CancellationToken cancellationToken)
    {
        ErroPagina = null;
        if (id <= 0)
        {
            ErroPagina = "Categoria inválida.";
            await CarregarCategoriasAsync(cancellationToken);
            return Page();
        }

        try
        {
            var res = await _api.DeleteCategoryAsync(id, cancellationToken);
            if (res.IsSuccessStatusCode)
            {
                await CarregarCategoriasAsync(cancellationToken);
                return Page();
            }

            var body = await res.Content.ReadAsStringAsync(cancellationToken);
            ErroPagina = res.StatusCode == HttpStatusCode.Conflict
                ? "Não é possível excluir: existem transações vinculadas a esta categoria."
                : $"Erro ao excluir ({(int)res.StatusCode}): {body}";
        }
        catch (Exception ex)
        {
            ErroPagina = ex.Message;
        }

        await CarregarCategoriasAsync(cancellationToken);
        return Page();
    }

    private async Task CarregarCategoriasAsync(CancellationToken cancellationToken)
    {
        Categorias = [];
        try
        {
            var res = await _api.GetCategoriesAsync(cancellationToken);
            if (res.IsSuccessStatusCode)
            {
                UsandoDadosDemo = false;
                await using var stream = await res.Content.ReadAsStreamAsync(cancellationToken);
                var arr = await JsonSerializer.DeserializeAsync<List<CategoryJson>>(stream, JsonOpts, cancellationToken)
                          ?? [];
                foreach (var c in arr.OrderBy(x => x.CategoryName))
                {
                    Categorias.Add(new CategoriaVm(
                        c.CategoryId,
                        PickIcon(c.CategoryName),
                        c.CategoryName ?? "—",
                        c.Description));
                }

                return;
            }
        }
        catch
        {
            /* fallback */
        }

        UsandoDadosDemo = true;
        EnsureDemoFallback();
        Categorias = _demoFallback!.ToList();
    }

    private static void EnsureDemoFallback()
    {
        lock (SeedLock)
        {
            _demoFallback ??=
            [
                new(null, "💼", "Salário", "Receitas fixas"),
                new(null, "📈", "Investimentos", "Rendimentos e resgates"),
                new(null, "🧳", "Freelance", "Projetos avulsos"),
                new(null, "🏠", "Moradia", "Aluguel, condominio, contas"),
                new(null, "🛒", "Alimentacao", "Mercado e refeicoes"),
                new(null, "🚗", "Transporte", "Combustivel e mobilidade"),
            ];
        }
    }

    private static string PickIcon(string? name)
    {
        var icons = new[] { "💼", "📈", "🏠", "🛒", "💰", "🎮" };
        if (string.IsNullOrEmpty(name)) return icons[0];
        return icons[Math.Abs(name.GetHashCode()) % icons.Length];
    }

    private sealed class CategoryJson
    {
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? Description { get; set; }
    }
}

public sealed record CategoriaVm(int? CategoryId, string Icone, string Nome, string? Descricao);
