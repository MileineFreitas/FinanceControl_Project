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
using FinanceControl.Web.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace FinanceControl.Web.Controllers.Reports;

[Route("relatorios")]
public class ReportsController(
    ITransactionCliService transactionCli,
    ICategoryCliService categoryCli,
    IPaymentMethodCliService paymentMethodCli,
    IStringLocalizer<SharedResources> localizer) : Controller
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
        vm.PageTitle = localizer["Reports.ByCategoryTitle"].Value;
        vm.PageSubtitle = localizer["Reports.ByCategorySubtitle"].Value;
        vm.DoughnutTitle = localizer["Reports.ExpensesByCategory"].Value;
        vm.DoughnutSubtitle = localizer["Reports.ExpensesByCategorySub"].Value;
        vm.BarTitle = localizer["Reports.IncomeVsExpenses"].Value;
        vm.BarSubtitle = localizer["Reports.IncomeVsExpensesSubCategory"].Value;
        vm.TableTitle = localizer["Reports.DetailByCategory"].Value;
        vm.TableSubtitle = localizer["Reports.DetailSub"].Value;
        vm.GroupColumnHeader = localizer["Common.Category"].Value;

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
        vm.PageTitle = localizer["Reports.ByPaymentTitle"].Value;
        vm.PageSubtitle = localizer["Reports.ByPaymentSubtitle"].Value;
        vm.DoughnutTitle = localizer["Reports.ExpensesByPayment"].Value;
        vm.DoughnutSubtitle = localizer["Reports.ExpensesByPaymentSub"].Value;
        vm.BarTitle = localizer["Reports.IncomeVsExpenses"].Value;
        vm.BarSubtitle = localizer["Reports.IncomeVsExpensesSubPayment"].Value;
        vm.TableTitle = localizer["Reports.DetailByPayment"].Value;
        vm.TableSubtitle = localizer["Reports.DetailSub"].Value;
        vm.GroupColumnHeader = localizer["Reports.PaymentMethodColumn"].Value;

        var (transactions, _) = await LoadTransactionsAndCategoriesAsync(vm);
        IReadOnlyList<PaymentMethodDto> paymentMethods = [];

        try
        {
            paymentMethods = await paymentMethodCli.ListAsync(includeInactive: true) ?? [];
        }
        catch
        {
            vm.ApiMensagem ??= localizer["Messages.ApiLoadPaymentMethods"].Value;
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
        var incomeLabel = localizer["Type.Income"].Value;
        var expenseLabel = localizer["Type.Expense"].Value;

        try
        {
            var txData = await transactionCli.ListAsync(new DataFilterDto { Page = 1, PageSize = 500 });
            if (txData?.Result == null)
                vm.ApiMensagem = localizer["Messages.ApiLoadTxError"].Value;
            else
                ReportDataBuilder.FillTransactionReport(vm, reference, txData.Result, fmt, incomeLabel, expenseLabel);
        }
        catch
        {
            vm.ApiMensagem = localizer["Messages.ApiConnectError"].Value;
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
                vm.ApiMensagem = localizer["Messages.ApiLoadTxError"].Value;
        }
        catch
        {
            vm.ApiMensagem = localizer["Messages.ApiConnectError"].Value;
        }

        try
        {
            var catData = await categoryCli.ListAsync(new DataFilterDto { Page = 1, PageSize = 200 });
            categories = catData?.Result ?? [];
        }
        catch
        {
            vm.ApiMensagem ??= localizer["Messages.ApiLoadCategories"].Value;
        }

        return (transactions, categories);
    }

    private string ResolveCategoryName(
        Guid categoryId,
        IReadOnlyDictionary<Guid, CategoryDto> lookup,
        IReadOnlyList<TransactionDto>? txs)
    {
        if (lookup.TryGetValue(categoryId, out var cat) && !string.IsNullOrWhiteSpace(cat.CategoryName))
            return cat.CategoryName.Trim();

        var fromTx = txs?.Select(t => t.CategoryName).FirstOrDefault(n => !string.IsNullOrWhiteSpace(n));
        if (!string.IsNullOrWhiteSpace(fromTx))
            return fromTx.Trim();

        return localizer["Reports.NoCategory"].Value;
    }

    private string ResolvePaymentMethodName(
        Guid paymentMethodId,
        IReadOnlyDictionary<Guid, PaymentMethodDto> lookup,
        IReadOnlyList<TransactionDto>? txs)
    {
        if (lookup.TryGetValue(paymentMethodId, out var pm) && !string.IsNullOrWhiteSpace(pm.Name))
            return pm.Name.Trim();

        var fromTx = txs?.Select(t => t.PaymentMethodName).FirstOrDefault(n => !string.IsNullOrWhiteSpace(n));
        if (!string.IsNullOrWhiteSpace(fromTx))
            return fromTx.Trim();

        return localizer["Reports.NoPaymentMethod"].Value;
    }
}
