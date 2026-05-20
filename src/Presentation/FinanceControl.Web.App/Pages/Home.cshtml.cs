using System.Globalization;
using System.Text.Json;
using FinanceControl.Web.Models.ViewModels;
using FinanceControl.Web.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FinanceControl.Web.Pages;

public class HomeModel : PageModel
{
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    private readonly IFinanceControlApiClient _api;

    public HomeModel(IFinanceControlApiClient api) => _api = api;

    public IReadOnlyList<DashboardMetricVm> Metrics { get; private set; } = [];

    public IReadOnlyList<DashboardTxRowVm> TransacoesResumo { get; private set; } = [];

    public int TotalTransacoesExemplo { get; private set; }

    public int TransacoesMostradas { get; private set; }

    public string? ApiMensagem { get; private set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        List<TxJson>? txs = null;
        try
        {
            var resTx = await _api.GetTransactionsAsync(cancellationToken);
            if (resTx.IsSuccessStatusCode)
            {
                await using var stream = await resTx.Content.ReadAsStreamAsync(cancellationToken);
                txs = await JsonSerializer.DeserializeAsync<List<TxJson>>(stream, JsonOpts, cancellationToken);
            }
        }
        catch
        {
            ApiMensagem = "Não foi possível carregar dados da API — exibindo valores de exemplo.";
        }

        if (txs is not { Count: > 0 })
        {
            CarregarMetricasEListaExemplo();
            return;
        }

        var nomePorCategoriaId = await CarregarNomesCategoriasAsync(cancellationToken);

        var culture = CultureInfo.GetCultureInfo("pt-BR");
        var now = DateTime.UtcNow;
        var inicioMes = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var fimMes = inicioMes.AddMonths(1);

        decimal gastosMes = 0, receitasMes = 0;
        foreach (var t in txs)
        {
            if (t.Date < inicioMes || t.Date >= fimMes)
                continue;
            if (t.TransactionTypeId == 2)
                gastosMes += t.TransactionValue;
            else if (t.TransactionTypeId == 1)
                receitasMes += t.TransactionValue;
        }

        decimal saldoConta = receitasMes - gastosMes;
        try
        {
            var accRes = await _api.GetAccountByIdAsync(1, cancellationToken);
            if (accRes.IsSuccessStatusCode)
            {
                await using var s = await accRes.Content.ReadAsStreamAsync(cancellationToken);
                var acc = await JsonSerializer.DeserializeAsync<AccountJson>(s, JsonOpts, cancellationToken);
                if (acc != null)
                    saldoConta = acc.CurrentBalance;
            }
        }
        catch
        {
            /* mantém estimativa */
        }

        Metrics =
        [
            new DashboardMetricVm(
                "Gasto total no mês",
                "R$ " + gastosMes.ToString("N2", culture),
                "Despesas (tipo saída) no mês vigente",
                true,
                "chart"),
            new DashboardMetricVm(
                "Receitas no mês",
                "R$ " + receitasMes.ToString("N2", culture),
                "Entradas confirmadas no período",
                false,
                "bank"),
            new DashboardMetricVm(
                "Saldo conta principal",
                "R$ " + saldoConta.ToString("N2", culture),
                "Após lançamentos com status pago",
                saldoConta < 0,
                "rocket"),
        ];

        var ordered = txs.OrderByDescending(x => x.Date).ThenByDescending(x => x.TransactionId).ToList();
        TotalTransacoesExemplo = ordered.Count;
        var top = ordered.Take(8).ToList();
        TransacoesMostradas = top.Count;

        TransacoesResumo = top.Select(t =>
        {
            var nomeCat = ResolverNomeCategoria(t, nomePorCategoriaId);
            var catNome = nomeCat.ToUpperInvariant();
            var isRec = t.TransactionTypeId == 1;
            var abs = Math.Abs(t.TransactionValue);
            var valorFmt = (isRec ? "+ R$ " : "- R$ ") + abs.ToString("N2", culture);
            var sub = $"#{t.TransactionId} • Conta {t.AccountId}";
            var dataFmt = t.Date.ToString("dd MMM, yyyy HH:mm", culture);
            return new DashboardTxRowVm(
                dataFmt,
                t.TransactionDescription ?? "—",
                sub,
                catNome,
                valorFmt,
                isRec,
                IconForCategory(nomeCat));
        }).ToList();
    }

    private static string ResolverNomeCategoria(TxJson t, IReadOnlyDictionary<int, string> nomePorCategoriaId)
    {
        if (!string.IsNullOrWhiteSpace(t.Category?.CategoryName))
            return t.Category.CategoryName.Trim();
        if (nomePorCategoriaId.TryGetValue(t.CategoryId, out var nome) && !string.IsNullOrWhiteSpace(nome))
            return nome.Trim();
        return "Categoria";
    }

    private async Task<Dictionary<int, string>> CarregarNomesCategoriasAsync(CancellationToken cancellationToken)
    {
        var map = new Dictionary<int, string>();
        try
        {
            var res = await _api.GetCategoriesAsync(cancellationToken);
            if (!res.IsSuccessStatusCode)
                return map;

            await using var stream = await res.Content.ReadAsStreamAsync(cancellationToken);
            var arr = await JsonSerializer.DeserializeAsync<List<CategoryRowJson>>(stream, JsonOpts, cancellationToken);
            if (arr == null)
                return map;
            foreach (var c in arr)
                map[c.CategoryId] = c.CategoryName?.Trim() ?? $"#{c.CategoryId}";
        }
        catch
        {
            /* ignore */
        }

        return map;
    }

    private void CarregarMetricasEListaExemplo()
    {
        Metrics =
        [
            new DashboardMetricVm(
                "Gasto total no mês",
                "R$ 4.280,50",
                "↑ 12% em relação ao mês anterior",
                true,
                "chart"),
            new DashboardMetricVm(
                "Saldo líquido de entrada",
                "R$ 12.540,00",
                "↓ Fluxo de caixa saudável",
                false,
                "bank"),
            new DashboardMetricVm(
                "Investimentos / poupança",
                "R$ 184.920,33",
                "↗ +8,4% Rendimento anualizado",
                false,
                "rocket"),
        ];

        TransacoesResumo =
        [
            new("12 out., 2023 14:30", "Apple Store — iPhone Pro", "#882910 • Cartão final 4829", "TECNOLOGIA", "- R$ 1.299,00", false, "shopping_bag"),
            new("11 out., 2023 09:15", "Transferência recebida — Freelance", "PIX • Conta corrente", "RENDA", "+ R$ 4.500,00", true, "payments"),
            new("10 out., 2023 19:42", "Supermercado Central", "Débito automático", "ALIMENTAÇÃO", "- R$ 287,45", false, "shopping_cart"),
            new("09 out., 2023 11:00", "Netflix assinatura", "Renovação mensal", "LAZER", "- R$ 55,90", false, "movie"),
            new("08 out., 2023 16:20", "Dividendos ITUB4", "Corretora XP • Conta investimento", "INVESTIMENTOS", "+ R$ 612,00", true, "show_chart"),
        ];

        TotalTransacoesExemplo = 42;
        TransacoesMostradas = TransacoesResumo.Count;
    }

    private static string IconForCategory(string? cat)
    {
        if (string.IsNullOrWhiteSpace(cat)) return "receipt_long";
        var s = cat.Trim();
        if (s.Equals("Salário", StringComparison.OrdinalIgnoreCase)) return "payments";
        if (s.Equals("Investimentos", StringComparison.OrdinalIgnoreCase)) return "show_chart";
        if (s.Equals("Moradia", StringComparison.OrdinalIgnoreCase)) return "home";
        if (s.Equals("Alimentação", StringComparison.OrdinalIgnoreCase)) return "shopping_cart";
        if (s.Equals("Lazer", StringComparison.OrdinalIgnoreCase)) return "movie";
        return "receipt_long";
    }

    private sealed class TxJson
    {
        public int TransactionId { get; set; }
        public string? TransactionDescription { get; set; }
        public decimal TransactionValue { get; set; }
        public DateTime Date { get; set; }
        public int TransactionTypeId { get; set; }
        public int CategoryId { get; set; }
        public int AccountId { get; set; }
        public CategoryMini? Category { get; set; }
    }

    private sealed class CategoryMini
    {
        public string? CategoryName { get; set; }
    }

    private sealed class CategoryRowJson
    {
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
    }

    private sealed class AccountJson
    {
        public decimal CurrentBalance { get; set; }
    }
}
