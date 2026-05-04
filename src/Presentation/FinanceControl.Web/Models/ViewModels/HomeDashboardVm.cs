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

/// <summary>Métricas do dashboard (cards superiores).</summary>
public sealed record DashboardMetricVm(
    string Title,
    string Value,
    string TrendText,
    bool TrendNegative,
    string IconSvgClass);

/// <summary>Linha da tabela “Resumo de transações” na home.</summary>
public sealed record DashboardTxRowVm(
    string DataHora,
    string Titulo,
    string LinhaSecundaria,
    string CategoriaBadge,
    string Valor,
    bool IsReceita,
    string IconMaterial);
