using FinanceControl.Client.Services.Interfaces;
using FinanceControl.Client.Services.Interfaces.Accounts;
using FinanceControl.Contracts.Dtos.Common;
using FinanceControl.Contracts.Enumerators.Transactions;
using FinanceControl.Web.Models.ViewModels;
using FinanceControl.Web.Models.ViewModels.Home;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text.Json;

namespace FinanceControl.Web.Controllers.Home;

[Route("home")]
public class HomeController : Controller
{
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };
    private static readonly CultureInfo PtBr = CultureInfo.GetCultureInfo("pt-BR");

    private readonly IFinanceControlApiClient _api;
    private readonly IAccountCliService _accountCli;

    public HomeController(IFinanceControlApiClient api, IAccountCliService accountCli)
    {
        _api = api;
        _accountCli = accountCli;
    }

    [HttpGet("")]
    [HttpGet("Index")]
    public async Task<IActionResult> Index()
    {
        var vm = new HomeIndexViewModel();
        List<TxJson>? txs = null;
        try
        {
            var resTx = await _api.GetTransactionsAsync();
            if (resTx.IsSuccessStatusCode)
            {
                await using var stream = await resTx.Content.ReadAsStreamAsync();
                var data = await JsonSerializer.DeserializeAsync<DataResultDto<TxJson>>(stream, JsonOpts);
                txs = data?.Result;
            }
            else
            {
                vm.ApiMensagem = "Não foi possível carregar transações da API.";
            }
        }
        catch
        {
            vm.ApiMensagem = "Não foi possível conectar à API. Verifique se o serviço está em execução.";
        }

        if (txs is not { Count: > 0 })
        {
            await PreencherEstadoVazioAsync(vm);
            return View("Index", vm);
        }

        var nomePorCategoriaId = await CarregarNomesCategoriasAsync();

        var now = DateTime.UtcNow;
        var inicioMes = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var fimMes = inicioMes.AddMonths(1);

        decimal gastosMes = 0, receitasMes = 0;
        foreach (var t in txs)
        {
            if (t.Date < inicioMes || t.Date >= fimMes)
                continue;
            if (t.TransactionTypeKind == TransactionTypeKind.Despesa)
                gastosMes += t.TransactionValue;
            else if (t.TransactionTypeKind == TransactionTypeKind.Receita)
                receitasMes += t.TransactionValue;
        }

        var saldoConta = await ObterSaldoTotalContasAsync();

        vm.Metrics =
        [
            new DashboardMetricVm(
                "Gasto total no mês",
                gastosMes.ToString("C", PtBr),
                "Despesas no mês vigente",
                true,
                "chart"),
            new DashboardMetricVm(
                "Receitas no mês",
                receitasMes.ToString("C", PtBr),
                "Entradas no mês vigente",
                false,
                "bank"),
            new DashboardMetricVm(
                "Saldo em contas",
                saldoConta.ToString("C", PtBr),
                "Soma do saldo atual das contas",
                saldoConta < 0,
                "rocket"),
        ];

        var ordered = txs.OrderByDescending(x => x.Date).ThenByDescending(x => x.TransactionId).ToList();
        vm.TotalTransacoes = ordered.Count;
        var top = ordered.Take(8).ToList();
        vm.TransacoesMostradas = top.Count;

        vm.TransacoesResumo = top.Select(t =>
        {
            var nomeCat = ResolverNomeCategoria(t, nomePorCategoriaId);
            var catNome = nomeCat.ToUpperInvariant();
            var isRec = t.TransactionTypeKind == TransactionTypeKind.Receita;
            var abs = Math.Abs(t.TransactionValue);
            var valorFmt = (isRec ? "+ " : "- ") + abs.ToString("C", PtBr);
            var sub = $"Conta {t.AccountId}";
            var dataFmt = t.Date.ToString("dd MMM, yyyy HH:mm", PtBr);
            return new DashboardTxRowVm(
                dataFmt,
                t.TransactionDescription ?? "—",
                sub,
                catNome,
                valorFmt,
                isRec,
                IconForCategory(nomeCat));
        }).ToList();

        return View("Index", vm);
    }

    private async Task PreencherEstadoVazioAsync(HomeIndexViewModel vm)
    {
        var saldoConta = await ObterSaldoTotalContasAsync();

        vm.Metrics =
        [
            new DashboardMetricVm("Gasto total no mês", 0m.ToString("C", PtBr), "—", false, "chart"),
            new DashboardMetricVm("Receitas no mês", 0m.ToString("C", PtBr), "—", false, "bank"),
            new DashboardMetricVm(
                "Saldo em contas",
                saldoConta.ToString("C", PtBr),
                saldoConta == 0 ? "Cadastre uma conta para começar" : "Saldo atual das contas",
                saldoConta < 0,
                "rocket"),
        ];
    }

    private async Task<decimal> ObterSaldoTotalContasAsync()
    {
        try
        {
            var contas = await _accountCli.ListAsync();
            return contas?.Sum(c => c.CurrentBalance) ?? 0m;
        }
        catch
        {
            return 0m;
        }
    }

    private static string ResolverNomeCategoria(TxJson t, IReadOnlyDictionary<Guid, string> nomePorCategoriaId)
    {
        if (!string.IsNullOrWhiteSpace(t.Category?.CategoryName))
            return t.Category.CategoryName.Trim();
        if (nomePorCategoriaId.TryGetValue(t.CategoryId, out var nome) && !string.IsNullOrWhiteSpace(nome))
            return nome.Trim();
        return "Categoria";
    }

    private async Task<Dictionary<Guid, string>> CarregarNomesCategoriasAsync()
    {
        var map = new Dictionary<Guid, string>();
        try
        {
            var res = await _api.GetCategoriesAsync();
            if (!res.IsSuccessStatusCode)
                return map;

            await using var stream = await res.Content.ReadAsStreamAsync();
            var data = await JsonSerializer.DeserializeAsync<DataResultDto<CategoryRowJson>>(stream, JsonOpts);
            if (data?.Result == null)
                return map;
            foreach (var c in data.Result)
                map[c.CategoryId] = c.CategoryName?.Trim() ?? $"#{c.CategoryId}";
        }
        catch
        {
            /* ignore */
        }

        return map;
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
        public Guid TransactionId { get; set; }
        public string? TransactionDescription { get; set; }
        public decimal TransactionValue { get; set; }
        public DateTime Date { get; set; }
        public TransactionTypeKind TransactionTypeKind { get; set; }
        public Guid CategoryId { get; set; }
        public Guid AccountId { get; set; }
        public CategoryMini? Category { get; set; }
    }

    private sealed class CategoryMini
    {
        public string? CategoryName { get; set; }
    }

    private sealed class CategoryRowJson
    {
        public Guid CategoryId { get; set; }
        public string? CategoryName { get; set; }
    }
}
