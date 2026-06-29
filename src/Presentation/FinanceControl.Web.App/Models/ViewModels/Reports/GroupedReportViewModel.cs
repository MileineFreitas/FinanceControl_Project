namespace FinanceControl.Web.Models.ViewModels.Reports;

public sealed class GroupedReportViewModel
{
    public string PageTitle { get; set; } = string.Empty;

    public string PageSubtitle { get; set; } = string.Empty;

    public string DoughnutTitle { get; set; } = string.Empty;

    public string DoughnutSubtitle { get; set; } = string.Empty;

    public string BarTitle { get; set; } = string.Empty;

    public string BarSubtitle { get; set; } = string.Empty;

    public string TableTitle { get; set; } = string.Empty;

    public string TableSubtitle { get; set; } = string.Empty;

    public string GroupColumnHeader { get; set; } = string.Empty;

    public string SelectedMonth { get; set; } = string.Empty;

    public string PeriodoLabel { get; set; } = string.Empty;

    public string TotalReceitasFmt { get; set; } = "R$ 0,00";

    public string TotalDespesasFmt { get; set; } = "R$ 0,00";

    public string SaldoFmt { get; set; } = "R$ 0,00";

    public bool SaldoNegativo { get; set; }

    public bool TemDados { get; set; }

    public string? ApiMensagem { get; set; }

    public IReadOnlyList<ReportMonthOptionVm> MonthOptions { get; set; } = [];

    public IReadOnlyList<GroupedReportRowVm> Rows { get; set; } = [];

    public string ChartDespesasJson { get; set; } = "{}";

    public string ChartComparativoJson { get; set; } = "{}";

    public string Idioma { get; set; } = "pt-BR";

    public string Moeda { get; set; } = "BRL";
}

public sealed record ReportMonthOptionVm(string Value, string Label, bool Selected);

public sealed record GroupedReportRowVm(
    Guid GroupId,
    string GroupName,
    string Icon,
    decimal Receita,
    decimal Despesa,
    decimal Saldo,
    int TransactionCount,
    decimal DespesaPercent,
    decimal ReceitaPercent,
    string ReceitaFmt,
    string DespesaFmt,
    string SaldoFmt);

public sealed class TransactionReportViewModel
{
    public string SelectedMonth { get; set; } = string.Empty;

    public string PeriodoLabel { get; set; } = string.Empty;

    public string TotalReceitasFmt { get; set; } = "R$ 0,00";

    public string TotalDespesasFmt { get; set; } = "R$ 0,00";

    public string SaldoFmt { get; set; } = "R$ 0,00";

    public int TotalTransacoes { get; set; }

    public bool SaldoNegativo { get; set; }

    public bool TemDados { get; set; }

    public string? ApiMensagem { get; set; }

    public IReadOnlyList<ReportMonthOptionVm> MonthOptions { get; set; } = [];

    public IReadOnlyList<TransactionReportRowVm> Rows { get; set; } = [];

    public string ChartTipoJson { get; set; } = "{}";

    public string ChartDiarioJson { get; set; } = "{}";

    public string Idioma { get; set; } = "pt-BR";

    public string Moeda { get; set; } = "BRL";
}

public sealed record TransactionReportRowVm(
    DateTime Date,
    string DateFmt,
    string Description,
    string CategoryName,
    string PaymentMethodName,
    bool IsReceita,
    string TypeLabel,
    decimal Value,
    string ValueFmt);
