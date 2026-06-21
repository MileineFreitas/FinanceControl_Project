namespace FinanceControl.Web.Models.ViewModels;

public sealed record SpendingRow(string Category, string Amount, string BarClass);

public sealed record TransactionRow(
    string Name,
    string Category,
    string Date,
    string Icon,
    string Amount,
    string Type,
    string TransClass,
    string IconClass,
    string AmountClass);

public sealed record DashboardMetricVm(
    string Title,
    string Value,
    string TrendText,
    bool TrendNegative,
    string IconSvgClass,
    bool IsSaldo = false);

public sealed record DashboardTxRowVm(
    string DataHora,
    long DataOrdenacao,
    string Titulo,
    string LinhaSecundaria,
    string CategoriaBadge,
    string Valor,
    decimal ValorOrdenacao,
    bool IsReceita,
    string IconMaterial);
