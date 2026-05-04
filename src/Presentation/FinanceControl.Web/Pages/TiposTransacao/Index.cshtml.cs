using System.Text.Json;
using FinanceControl.Domain.Entities;
using FinanceControl.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FinanceControl.Web.Pages.TiposTransacao;

public class IndexModel : PageModel
{
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };
    private static readonly object DemoLock = new();
    private static List<MeioPagamentoVm>? _demoFallback;

    private readonly IFinanceControlApiClient _api;

    public IndexModel(IFinanceControlApiClient api) => _api = api;

    [BindProperty(SupportsGet = true)]
    public string? Abrir { get; set; }

    [BindProperty]
    public PaymentMethodRegisterDto PaymentMethodInput { get; set; } = new();

    [BindProperty]
    public string SelectedIcon { get; set; } = "💳";

    public bool ModalAberto { get; set; }

    public string? ErroModal { get; set; }

    public string? ErroPagina { get; set; }

    public bool UsandoDadosDemo { get; set; }

    public List<MeioPagamentoVm> Meios { get; private set; } = [];

    public IReadOnlyList<string> IconesDisponiveis { get; } =
    [
        "💳", "📱", "🏦", "💵", "🧾", "📲", "🏧", "💎",
        "🎫", "🛒", "✈️", "🚗", "🏪", "💰", "🔐", "📮",
    ];

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        if (string.Equals(Abrir?.Trim(), "novo", StringComparison.OrdinalIgnoreCase))
            ModalAberto = true;

        await CarregarMeiosAsync(cancellationToken);
    }

    public async Task<IActionResult> OnPostCreatePaymentMethodAsync(CancellationToken cancellationToken)
    {
        ModalAberto = true;
        await CarregarMeiosAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(PaymentMethodInput.Name) || PaymentMethodInput.Name.Trim().Length < 2)
        {
            ErroModal = "Nome deve ter pelo menos 2 caracteres.";
            return Page();
        }

        try
        {
            var response = await _api.RegisterPaymentMethodAsync(PaymentMethodInput, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                ErroModal = $"Erro ao salvar: {await response.Content.ReadAsStringAsync(cancellationToken)}";
                return Page();
            }

            ModalAberto = false;
            ErroModal = null;
            PaymentMethodInput = new PaymentMethodRegisterDto();
            SelectedIcon = "💳";
            return RedirectToPage("/tipos-transacao");
        }
        catch (Exception ex)
        {
            ErroModal = $"Erro inesperado: {ex.Message}";
            return Page();
        }
    }

    public async Task<IActionResult> OnPostDeletePaymentMethodAsync(int id, CancellationToken cancellationToken)
    {
        ErroPagina = null;
        if (id <= 0)
        {
            ErroPagina = "Registro inválido.";
            await CarregarMeiosAsync(cancellationToken);
            return Page();
        }

        try
        {
            var res = await _api.DeletePaymentMethodAsync(id, cancellationToken);
            if (res.IsSuccessStatusCode)
            {
                await CarregarMeiosAsync(cancellationToken);
                return Page();
            }

            var body = await res.Content.ReadAsStringAsync(cancellationToken);
            ErroPagina = $"Erro ao excluir ({(int)res.StatusCode}): {body}";
        }
        catch (Exception ex)
        {
            ErroPagina = ex.Message;
        }

        await CarregarMeiosAsync(cancellationToken);
        return Page();
    }

    private async Task CarregarMeiosAsync(CancellationToken cancellationToken)
    {
        Meios = [];
        try
        {
            var res = await _api.GetPaymentMethodsAsync(cancellationToken);
            if (res.IsSuccessStatusCode)
            {
                UsandoDadosDemo = false;
                await using var stream = await res.Content.ReadAsStreamAsync(cancellationToken);
                var arr = await JsonSerializer.DeserializeAsync<List<PaymentMethodJson>>(stream, JsonOpts, cancellationToken)
                          ?? [];
                foreach (var p in arr.OrderBy(x => x.Name))
                {
                    Meios.Add(new MeioPagamentoVm(
                        p.PaymentMethodId,
                        PickIcon(p.Name),
                        p.Name ?? "—",
                        p.Description));
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
        Meios = _demoFallback!.ToList();
    }

    private static void EnsureDemoFallback()
    {
        lock (DemoLock)
        {
            _demoFallback ??=
            [
                new(null, "💳", "Cartão de crédito", "Visa/Master — fatura mensal"),
                new(null, "📱", "PIX", "Transferências instantâneas"),
                new(null, "💵", "Dinheiro", "Caixa físico"),
                new(null, "🏦", "Débito em conta", "TED / débito automático"),
                new(null, "🧾", "Boleto", "Pagamentos agendados"),
            ];
        }
    }

    private static string PickIcon(string? name)
    {
        var icons = new[] { "💳", "📱", "🏦", "💵", "🧾", "📲", "💰" };
        if (string.IsNullOrEmpty(name)) return icons[0];
        return icons[Math.Abs(name.GetHashCode()) % icons.Length];
    }

    private sealed class PaymentMethodJson
    {
        public int PaymentMethodId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}

public sealed record MeioPagamentoVm(int? PaymentMethodId, string Icone, string Nome, string? Descricao);
