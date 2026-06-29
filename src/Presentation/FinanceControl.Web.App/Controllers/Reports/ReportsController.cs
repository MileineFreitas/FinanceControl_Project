using FinanceControl.Client.Services.Interfaces.Categories;
using FinanceControl.Client.Services.Interfaces.PaymentMethods;
using FinanceControl.Client.Services.Interfaces.Transactions;
using FinanceControl.Contracts.Constants;
using FinanceControl.Contracts.Dtos.Categories;
using FinanceControl.Contracts.Dtos.PaymentMethods;
using FinanceControl.Contracts.Dtos.Transactions;
using FinanceControl.Contracts.Filters;
using FinanceControl.Web.Helpers;
using FinanceControl.Web.Models.ViewModels.Reports;
using Microsoft.AspNetCore.Mvc;

namespace FinanceControl.Web.Controllers.Reports;

[Route("relatorios")]
public class ReportsController(
    ITransactionCliService transactionCli,
    ICategoryCliService categoryCli,
    IPaymentMethodCliService paymentMethodCli) : Controller
{
    [HttpGet("")]
    [HttpGet("Index")]
    public IActionResult Index() => RedirectToAction(nameof(PorCategoria));

    [HttpGet("por-meio-pagamento")]
    public async Task<IActionResult> PorMeioPagamento([FromQuery] string? mes) =>
        View("PorMeioPagamento", await BuildPaymentMethodReportAsync(mes));

    [HttpGet("por-categoria")]
    public async Task<IActionResult> PorCategoria([FromQuery] string? mes) =>
        View("PorCategoria", await BuildCategoryReportAsync(mes));

    [HttpGet("por-transacoes")]
    public async Task<IActionResult> PorTransacoes([FromQuery] string? mes) =>
        View("PorTransacoes", await BuildTransactionReportAsync(mes));

    private FinancialFormatContext GetFormatContext() =>
        FinancialFormatContext.From(User);

    private async Task<GroupedReportViewModel> BuildCategoryReportAsync(string? mes)
    {
        var fmt = GetFormatContext();
        var reference = ReportDataBuilder.ParseMonth(mes, fmt) ?? DateTime.UtcNow;
        var vm = ReportDataBuilder.CreateMonthShell(reference, fmt);
        vm.PageTitle = "Resumo por categoria";
        vm.PageSubtitle = "Distribuição de receitas e despesas conforme suas categorias cadastradas.";
        vm.DoughnutTitle = "Despesas por categoria";
        vm.DoughnutSubtitle = "Participação de cada categoria no total de saídas.";
        vm.BarTitle = "Receitas vs despesas";
        vm.BarSubtitle = "Comparativo das principais categorias do período.";
        vm.TableTitle = "Detalhamento por categoria";
        vm.TableSubtitle = "Valores absolutos e percentual sobre o total do mês.";
        vm.GroupColumnHeader = "Categoria";

        var (transactions, categories) = await LoadTransactionsAndCategoriesAsync(vm);
        var lookup = categories.ToDictionary(c => c.CategoryId);

        ReportDataBuilder.FillGroupedReport(
            vm,
            reference,
            transactions,
            lookup.Keys,
            t => t.CategoryId,
            (id, txs) => ResolveCategoryName(id, lookup, txs),
            id => lookup.TryGetValue(id, out var c) ? CategoryIcons.Normalize(c.Icon) : CategoryIcons.Default,
            fmt);

        return vm;
    }

    private async Task<GroupedReportViewModel> BuildPaymentMethodReportAsync(string? mes)
    {
        var fmt = GetFormatContext();
        var reference = ReportDataBuilder.ParseMonth(mes, fmt) ?? DateTime.UtcNow;
        var vm = ReportDataBuilder.CreateMonthShell(reference, fmt);
        vm.PageTitle = "Resumo por meio de pagamento";
        vm.PageSubtitle = "Distribuição de receitas e despesas conforme os meios de pagamento cadastrados.";
        vm.DoughnutTitle = "Despesas por meio de pagamento";
        vm.DoughnutSubtitle = "Participação de cada meio no total de saídas.";
        vm.BarTitle = "Receitas vs despesas";
        vm.BarSubtitle = "Comparativo dos principais meios de pagamento no período.";
        vm.TableTitle = "Detalhamento por meio de pagamento";
        vm.TableSubtitle = "Valores absolutos e percentual sobre o total do mês.";
        vm.GroupColumnHeader = "Meio de pagamento";

        var (transactions, _) = await LoadTransactionsAndCategoriesAsync(vm);
        IReadOnlyList<PaymentMethodDto> paymentMethods = [];

        try
        {
            paymentMethods = await paymentMethodCli.ListAsync(includeInactive: true) ?? [];
        }
        catch
        {
            vm.ApiMensagem ??= "Não foi possível carregar meios de pagamento da API.";
        }

        var lookup = paymentMethods.ToDictionary(p => p.PaymentMethodId);

        ReportDataBuilder.FillGroupedReport(
            vm,
            reference,
            transactions,
            lookup.Keys,
            t => t.PaymentMethodId,
            (id, txs) => ResolvePaymentMethodName(id, lookup, txs),
            id => lookup.TryGetValue(id, out var p) ? PaymentMethodIcons.Normalize(p.Icon) : PaymentMethodIcons.Default,
            fmt);

        return vm;
    }

    private async Task<TransactionReportViewModel> BuildTransactionReportAsync(string? mes)
    {
        var fmt = GetFormatContext();
        var reference = ReportDataBuilder.ParseMonth(mes, fmt) ?? DateTime.UtcNow;
        var vm = ReportDataBuilder.CreateTransactionMonthShell(reference, fmt);

        try
        {
            var txData = await transactionCli.ListAsync(new DataFilterDto { Page = 1, PageSize = 500 });
            if (txData?.Result == null)
                vm.ApiMensagem = "Não foi possível carregar transações da API.";
            else
                ReportDataBuilder.FillTransactionReport(vm, reference, txData.Result, fmt);
        }
        catch
        {
            vm.ApiMensagem = "Não foi possível conectar à API. Verifique se o serviço está em execução.";
        }

        return vm;
    }

    private async Task<(IReadOnlyList<TransactionDto> Transactions, IReadOnlyList<CategoryDto> Categories)> LoadTransactionsAndCategoriesAsync(GroupedReportViewModel vm)
    {
        IReadOnlyList<TransactionDto> transactions = [];
        IReadOnlyList<CategoryDto> categories = [];

        try
        {
            var txData = await transactionCli.ListAsync(new DataFilterDto { Page = 1, PageSize = 500 });
            transactions = txData?.Result ?? [];
            if (txData?.Result == null)
                vm.ApiMensagem = "Não foi possível carregar transações da API.";
        }
        catch
        {
            vm.ApiMensagem = "Não foi possível conectar à API. Verifique se o serviço está em execução.";
        }

        try
        {
            var catData = await categoryCli.ListAsync(new DataFilterDto { Page = 1, PageSize = 200 });
            categories = catData?.Result ?? [];
        }
        catch
        {
            vm.ApiMensagem ??= "Não foi possível carregar categorias da API.";
        }

        return (transactions, categories);
    }

    private static string ResolveCategoryName(
        Guid categoryId,
        IReadOnlyDictionary<Guid, CategoryDto> lookup,
        IReadOnlyList<TransactionDto>? txs)
    {
        if (lookup.TryGetValue(categoryId, out var cat) && !string.IsNullOrWhiteSpace(cat.CategoryName))
            return cat.CategoryName.Trim();

        var fromTx = txs?.Select(t => t.CategoryName).FirstOrDefault(n => !string.IsNullOrWhiteSpace(n));
        if (!string.IsNullOrWhiteSpace(fromTx))
            return fromTx.Trim();

        return "Sem categoria";
    }

    private static string ResolvePaymentMethodName(
        Guid paymentMethodId,
        IReadOnlyDictionary<Guid, PaymentMethodDto> lookup,
        IReadOnlyList<TransactionDto>? txs)
    {
        if (lookup.TryGetValue(paymentMethodId, out var pm) && !string.IsNullOrWhiteSpace(pm.Name))
            return pm.Name.Trim();

        var fromTx = txs?.Select(t => t.PaymentMethodName).FirstOrDefault(n => !string.IsNullOrWhiteSpace(n));
        if (!string.IsNullOrWhiteSpace(fromTx))
            return fromTx.Trim();

        return "Sem meio de pagamento";
    }
}
