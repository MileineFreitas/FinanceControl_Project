using FinanceControl.Domain.Entities;
using FinanceControl.Domain.Enums;
using FinanceControl.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FinanceControl.Web.Pages.Categories;

public class IndexModel : PageModel
{
    private static readonly object SeedLock = new();
    private static List<CategoriaVm>? _todas;

    private readonly IFinanceControlApiClient _api;

    public IndexModel(IFinanceControlApiClient api)
    {
        _api = api;
    }

    [BindProperty(SupportsGet = true)]
    public string Aba { get; set; } = "receitas";

    [BindProperty]
    public CategoryRegisterDto CategoryInput { get; set; } = new();

    [BindProperty]
    public string SelectedIcon { get; set; } = "💰";

    public bool ModalAberto { get; set; }

    public string? ErroModal { get; set; }

    public List<CategoriaVm> Categorias { get; private set; } = [];

    public IReadOnlyList<string> IconesDisponiveis { get; } =
    [
        "🍔", "🚗", "🏠", "💊", "📚", "🎮", "✈️", "👕", "💡", "🐶",
        "💰", "🎵", "🏋️", "🎁", "💼", "📈", "🧳", "🛒",
    ];

    public void OnGet()
    {
        EnsureSeed();
        Categorias = Filtradas();
    }

    public async Task<IActionResult> OnPostCreateCategoryAsync(CancellationToken cancellationToken)
    {
        EnsureSeed();

        ModalAberto = true;

        if (string.IsNullOrWhiteSpace(CategoryInput.CategoryName) || CategoryInput.CategoryName.Length < 2)
        {
            ErroModal = "Nome deve ter pelo menos 2 caracteres.";
            Categorias = Filtradas();
            return Page();
        }

        try
        {
            var response = await _api.RegisterCategoryAsync(CategoryInput, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                ErroModal = $"Erro ao salvar: {await response.Content.ReadAsStringAsync(cancellationToken)}";
                Categorias = Filtradas();
                return Page();
            }

            var tipo = CategoryInput.Type == TransactionType.Receita ? "receitas" : "custos";
            lock (SeedLock)
            {
                _todas!.Add(new CategoriaVm(SelectedIcon, CategoryInput.CategoryName, 0, 0, tipo));
            }

            ModalAberto = false;
            ErroModal = null;
            CategoryInput = new CategoryRegisterDto();
            SelectedIcon = "💰";
            Categorias = Filtradas();
            return Page();
        }
        catch (Exception ex)
        {
            ErroModal = $"Erro inesperado: {ex.Message}";
            Categorias = Filtradas();
            return Page();
        }
    }

    private List<CategoriaVm> Filtradas()
    {
        EnsureSeed();
        lock (SeedLock)
        {
            return _todas!.Where(c => c.Tipo == Aba).ToList();
        }
    }

    private static void EnsureSeed()
    {
        lock (SeedLock)
        {
            if (_todas != null) return;

            _todas =
            [
                new("💼", "Salário", 12450.00m, 75, "receitas"),
                new("📈", "Investimentos", 3120.40m, 40, "receitas"),
                new("🧳", "Freelance", 5800.00m, 55, "receitas"),
                new("🏠", "Moradia", 3200.00m, 60, "custos"),
                new("🛒", "Alimentação", 1450.00m, 35, "custos"),
                new("🚗", "Transporte", 890.00m, 25, "custos"),
            ];
        }
    }
}

public sealed record CategoriaVm(string Icone, string Nome, decimal Total, int Percentual, string Tipo);
