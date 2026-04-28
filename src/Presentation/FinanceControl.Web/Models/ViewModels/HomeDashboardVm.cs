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
