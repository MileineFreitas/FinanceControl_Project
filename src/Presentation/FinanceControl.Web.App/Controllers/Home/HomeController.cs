using FinanceControl.Client.Services.Interfaces.Accounts;
using FinanceControl.Client.Services.Interfaces.Categories;
using FinanceControl.Client.Services.Interfaces.Transactions;
using FinanceControl.Contracts.Dtos.Transactions;
using FinanceControl.Contracts.Enumerators.Transactions;
using FinanceControl.Contracts.Filters;
using FinanceControl.Web.Models.ViewModels;
using FinanceControl.Web.Models.ViewModels.Home;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace FinanceControl.Web.Controllers.Home;

[Route("home")]
public class HomeController(
    ITransactionCliService transactionCli,
    ICategoryCliService categoryCli,
    IAccountCliService accountCli) : Controller
{
    private static readonly CultureInfo PtBr = CultureInfo.GetCultureInfo("pt-BR");

    [HttpGet("")]
    [HttpGet("Index")]
    public async Task<IActionResult> Index()
    {
        var vm = new HomeIndexViewModel();
        IReadOnlyList<TransactionDto>? txs = null;
        try
        {
            var data = await transactionCli.ListAsync(new DataFilterDto { Page = 1, PageSize = 200 });
            txs = data?.Result;
            if (txs == null)
                vm.ApiMensagem = "Não foi possível carregar transações da API.";
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
            var valorOrdenacao = isRec ? abs : -abs;
            var valorFmt = (isRec ? "+ " : "- ") + abs.ToString("C", PtBr);
            var sub = string.IsNullOrWhiteSpace(t.AccountName) ? $"Conta {t.AccountId}" : t.AccountName;
            var dataUtc = t.Date.UtcDateTime;
            var dataFmt = dataUtc.ToString("dd MMM, yyyy HH:mm", PtBr);
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
            var contas = await accountCli.ListAsync();
            return contas?.Sum(c => c.CurrentBalance) ?? 0m;
        }
        catch
        {
            return 0m;
        }
    }

    private static string ResolverNomeCategoria(TransactionDto t, IReadOnlyDictionary<Guid, string> nomePorCategoriaId)
    {
        if (!string.IsNullOrWhiteSpace(t.CategoryName))
            return t.CategoryName.Trim();
        if (nomePorCategoriaId.TryGetValue(t.CategoryId, out var nome) && !string.IsNullOrWhiteSpace(nome))
            return nome.Trim();
        return "Categoria";
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
