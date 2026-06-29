using FinanceControl.Client.Services.Interfaces.Accounts;
using FinanceControl.Client.Services.Interfaces.Categories;
using FinanceControl.Client.Services.Interfaces.Transactions;
using FinanceControl.Contracts.Dtos.Transactions;
using FinanceControl.Contracts.Enumerators.Transactions;
using FinanceControl.Contracts.Filters;
using FinanceControl.Web.Helpers;
using FinanceControl.Web.Models.ViewModels;
using FinanceControl.Web.Models.ViewModels.Home;
using FinanceControl.Web.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace FinanceControl.Web.Controllers.Home;

[Route("home")]
public class HomeController(
    ITransactionCliService transactionCli,
    ICategoryCliService categoryCli,
    IAccountCliService accountCli,
    IStringLocalizer<SharedResources> localizer) : Controller
{
    [HttpGet("")]
    [HttpGet("Index")]
    public async Task<IActionResult> Index()
    {
        var fmt = FinancialFormatContext.From(User);
        var vm = new HomeIndexViewModel
        {
            Idioma = fmt.Idioma,
            Moeda = fmt.Moeda
        };
        IReadOnlyList<TransactionDto>? txs = null;
        try
        {
            var data = await transactionCli.ListAsync(new DataFilterDto { Page = 1, PageSize = 200 });
            txs = data?.Result;
            if (txs == null)
                vm.ApiMensagem = localizer["Messages.ApiLoadTxError"].Value;
        }
        catch
        {
            vm.ApiMensagem = localizer["Messages.ApiConnectError"].Value;
        }

        if (txs is not { Count: > 0 })
        {
            await PreencherEstadoVazioAsync(vm, fmt);
            return View("Index", vm);
        }

        var nomePorCategoriaId = await CarregarNomesCategoriasAsync();

        var now = DateTime.UtcNow;
        var (inicioMes, fimMes) = fmt.GetFinancialMonthRange(now);

        decimal gastosMes = 0, receitasMes = 0;
        foreach (var t in txs)
        {
            var date = t.Date.UtcDateTime;
            if (date < inicioMes || date >= fimMes)
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
                localizer["Home.MetricMonthlySpend"].Value,
                fmt.FormatCurrency(gastosMes),
                localizer["Home.TrendMonthlyExpenses"].Value,
                true,
                "chart"),
            new DashboardMetricVm(
                localizer["Home.MetricMonthlyIncome"].Value,
                fmt.FormatCurrency(receitasMes),
                localizer["Home.TrendMonthlyIncome"].Value,
                false,
                "bank"),
            new DashboardMetricVm(
                localizer["Home.MetricAccountBalance"].Value,
                fmt.FormatCurrency(saldoConta),
                localizer["Home.TrendAccountSum"].Value,
                saldoConta < 0,
                "rocket",
                IsSaldo: true),
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
            var valorOrdenacao = isRec ? abs : -abs;
            var valorFmt = fmt.FormatSignedCurrency(abs, isRec);
            var sub = string.IsNullOrWhiteSpace(t.AccountName)
                ? $"{localizer["Common.Account"].Value} {t.AccountId}"
                : t.AccountName;
            var dataUtc = t.Date.UtcDateTime;
            var dataFmt = fmt.FormatDateTimeLong(dataUtc);
            return new DashboardTxRowVm(
                dataFmt,
                dataUtc.Ticks,
                t.TransactionDescription ?? "—",
                sub,
                catNome,
                valorFmt,
                valorOrdenacao,
                isRec,
                IconForCategory(nomeCat));
        }).ToList();

        return View("Index", vm);
    }

    private async Task PreencherEstadoVazioAsync(HomeIndexViewModel vm, FinancialFormatContext fmt)
    {
        var saldoConta = await ObterSaldoTotalContasAsync();

        vm.Metrics =
        [
            new DashboardMetricVm(localizer["Home.MetricMonthlySpend"].Value, fmt.FormatCurrency(0), "—", false, "chart"),
            new DashboardMetricVm(localizer["Home.MetricMonthlyIncome"].Value, fmt.FormatCurrency(0), "—", false, "bank"),
            new DashboardMetricVm(
                localizer["Home.MetricAccountBalance"].Value,
                fmt.FormatCurrency(saldoConta),
                saldoConta == 0 ? localizer["Home.RegisterAccountHint"].Value : localizer["Home.CurrentBalance"].Value,
                saldoConta < 0,
                "rocket",
                IsSaldo: true),
        ];
    }

    private async Task<decimal> ObterSaldoTotalContasAsync()
    {
        try
        {
            var userId = User.GetUserId();
            var contas = await accountCli.ListAsync(userId);
            return contas?.Sum(c => c.CurrentBalance) ?? 0m;
        }
        catch
        {
            return 0m;
        }
    }

    private string ResolverNomeCategoria(TransactionDto t, IReadOnlyDictionary<Guid, string> nomePorCategoriaId)
    {
        if (!string.IsNullOrWhiteSpace(t.CategoryName))
            return t.CategoryName.Trim();
        if (nomePorCategoriaId.TryGetValue(t.CategoryId, out var nome) && !string.IsNullOrWhiteSpace(nome))
            return nome.Trim();
        return localizer["Common.Category"].Value;
    }

    private async Task<Dictionary<Guid, string>> CarregarNomesCategoriasAsync()
    {
        var map = new Dictionary<Guid, string>();
        try
        {
            var data = await categoryCli.ListAsync(new DataFilterDto { Page = 1, PageSize = 200 });
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
}
