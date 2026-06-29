using System.Globalization;
using System.Text.Json;
using FinanceControl.Contracts.Dtos.Transactions;
using FinanceControl.Contracts.Enumerators.Transactions;
using FinanceControl.Web.Models.ViewModels.Reports;

namespace FinanceControl.Web.Helpers;

internal static class ReportDataBuilder
{
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public static GroupedReportViewModel CreateMonthShell(DateTime reference, FinancialFormatContext fmt) =>
        new()
        {
            SelectedMonth = reference.ToString("yyyy-MM"),
            PeriodoLabel = reference.ToString("MMMM yyyy", fmt.Culture),
            MonthOptions = BuildMonthOptions(reference, fmt),
            Idioma = fmt.Idioma,
            Moeda = fmt.Moeda,
        };

    public static TransactionReportViewModel CreateTransactionMonthShell(DateTime reference, FinancialFormatContext fmt) =>
        new()
        {
            SelectedMonth = reference.ToString("yyyy-MM"),
            PeriodoLabel = reference.ToString("MMMM yyyy", fmt.Culture),
            MonthOptions = BuildMonthOptions(reference, fmt),
            Idioma = fmt.Idioma,
            Moeda = fmt.Moeda,
        };

    public static IReadOnlyList<TransactionDto> FilterByMonth(IReadOnlyList<TransactionDto> transactions, DateTime reference)
    {
        var inicio = MonthStart(reference);
        var fim = inicio.AddMonths(1);
        return transactions
            .Where(t =>
            {
                var d = t.Date.UtcDateTime;
                return d >= inicio && d < fim;
            })
            .ToList();
    }

    public static void FillGroupedReport(
        GroupedReportViewModel vm,
        DateTime reference,
        IReadOnlyList<TransactionDto> transactions,
        IEnumerable<Guid> registeredGroupIds,
        Func<TransactionDto, Guid> groupKeySelector,
        Func<Guid, IReadOnlyList<TransactionDto>?, string> resolveName,
        Func<Guid, string> resolveIcon,
        FinancialFormatContext fmt)
    {
        var noPeriodo = FilterByMonth(transactions, reference);
        var porGrupo = noPeriodo.GroupBy(groupKeySelector).ToDictionary(g => g.Key, g => g.ToList());

        var groupIds = new HashSet<Guid>(registeredGroupIds);
        foreach (var id in porGrupo.Keys)
            groupIds.Add(id);

        decimal totalReceitas = 0, totalDespesas = 0;
        var rows = new List<GroupedReportRowVm>();

        foreach (var groupId in groupIds.OrderBy(id => resolveName(id, null)))
        {
            porGrupo.TryGetValue(groupId, out var txs);
            var (receita, despesa) = SumByKind(txs);
            totalReceitas += receita;
            totalDespesas += despesa;

            rows.Add(new GroupedReportRowVm(
                groupId,
                resolveName(groupId, txs),
                resolveIcon(groupId),
                receita,
                despesa,
                receita - despesa,
                txs?.Count ?? 0,
                0,
                0,
                fmt.FormatCurrency(receita),
                fmt.FormatCurrency(despesa),
                fmt.FormatCurrency(receita - despesa)));
        }

        rows = rows
            .Select(r => r with
            {
                DespesaPercent = totalDespesas > 0 ? Math.Round(r.Despesa / totalDespesas * 100, 1) : 0,
                ReceitaPercent = totalReceitas > 0 ? Math.Round(r.Receita / totalReceitas * 100, 1) : 0,
            })
            .OrderByDescending(r => r.Despesa + r.Receita)
            .ThenBy(r => r.GroupName)
            .ToList();

        ApplyTotals(vm, totalReceitas, totalDespesas, noPeriodo.Count, fmt);
        vm.Rows = rows;
        vm.ChartDespesasJson = BuildDespesasChartJson(rows);
        vm.ChartComparativoJson = BuildComparativoChartJson(rows);
    }

    public static void FillTransactionReport(
        TransactionReportViewModel vm,
        DateTime reference,
        IReadOnlyList<TransactionDto> transactions,
        FinancialFormatContext fmt,
        string incomeLabel,
        string expenseLabel)
    {
        var noPeriodo = FilterByMonth(transactions, reference).OrderByDescending(t => t.Date).ToList();
        decimal totalReceitas = 0, totalDespesas = 0;

        foreach (var t in noPeriodo)
        {
            if (t.TransactionTypeKind == TransactionTypeKind.Receita)
                totalReceitas += t.TransactionValue;
            else if (t.TransactionTypeKind == TransactionTypeKind.Despesa)
                totalDespesas += t.TransactionValue;
        }

        var saldo = totalReceitas - totalDespesas;
        vm.TotalReceitasFmt = fmt.FormatCurrency(totalReceitas);
        vm.TotalDespesasFmt = fmt.FormatCurrency(totalDespesas);
        vm.SaldoFmt = fmt.FormatCurrency(saldo);
        vm.SaldoNegativo = saldo < 0;
        vm.TemDados = noPeriodo.Count > 0;
        vm.TotalTransacoes = noPeriodo.Count;

        vm.Rows = noPeriodo.Select(t =>
        {
            var isRec = t.TransactionTypeKind == TransactionTypeKind.Receita;
            var abs = Math.Abs(t.TransactionValue);
            var valueFmt = fmt.FormatSignedCurrency(abs, isRec);
            return new TransactionReportRowVm(
                t.Date.UtcDateTime,
                fmt.FormatDateTime(t.Date.UtcDateTime),
                string.IsNullOrWhiteSpace(t.TransactionDescription) ? "—" : t.TransactionDescription.Trim(),
                string.IsNullOrWhiteSpace(t.CategoryName) ? "—" : t.CategoryName.Trim(),
                string.IsNullOrWhiteSpace(t.PaymentMethodName) ? "—" : t.PaymentMethodName.Trim(),
                isRec,
                isRec ? incomeLabel : expenseLabel,
                t.TransactionValue,
                valueFmt);
        }).ToList();

        vm.ChartTipoJson = JsonSerializer.Serialize(new
        {
            labels = new[] { incomeLabel, expenseLabel },
            values = new[] { totalReceitas, totalDespesas },
        }, JsonOpts);

        vm.ChartDiarioJson = BuildDailyChartJson(noPeriodo, reference, fmt);
    }

    public static DateTime? ParseMonth(string? mes, FinancialFormatContext fmt)
    {
        if (string.IsNullOrWhiteSpace(mes))
            return null;

        if (DateTime.TryParseExact(mes.Trim(), "yyyy-MM", fmt.Culture, DateTimeStyles.None, out var parsed))
            return MonthStart(parsed);

        return null;
    }

    private static DateTime MonthStart(DateTime date) =>
        new(date.Year, date.Month, 1, 0, 0, 0, DateTimeKind.Utc);

    private static List<ReportMonthOptionVm> BuildMonthOptions(DateTime reference, FinancialFormatContext fmt)
    {
        var now = DateTime.UtcNow;
        var options = new List<ReportMonthOptionVm>();

        for (var i = 0; i < 12; i++)
        {
            var month = MonthStart(now).AddMonths(-i);
            options.Add(new ReportMonthOptionVm(
                month.ToString("yyyy-MM"),
                month.ToString("MMMM yyyy", fmt.Culture),
                month.Year == reference.Year && month.Month == reference.Month));
        }

        return options;
    }

    private static void ApplyTotals(GroupedReportViewModel vm, decimal totalReceitas, decimal totalDespesas, int count, FinancialFormatContext fmt)
    {
        var saldo = totalReceitas - totalDespesas;
        vm.TotalReceitasFmt = fmt.FormatCurrency(totalReceitas);
        vm.TotalDespesasFmt = fmt.FormatCurrency(totalDespesas);
        vm.SaldoFmt = fmt.FormatCurrency(saldo);
        vm.SaldoNegativo = saldo < 0;
        vm.TemDados = count > 0;
    }

    private static (decimal Receita, decimal Despesa) SumByKind(IReadOnlyList<TransactionDto>? txs)
    {
        decimal receita = 0, despesa = 0;
        if (txs == null) return (receita, despesa);

        foreach (var t in txs)
        {
            if (t.TransactionTypeKind == TransactionTypeKind.Receita)
                receita += t.TransactionValue;
            else if (t.TransactionTypeKind == TransactionTypeKind.Despesa)
                despesa += t.TransactionValue;
        }

        return (receita, despesa);
    }

    private static string BuildDespesasChartJson(IReadOnlyList<GroupedReportRowVm> rows)
    {
        var slices = rows
            .Where(r => r.Despesa > 0)
            .OrderByDescending(r => r.Despesa)
            .Select(r => new { label = $"{r.Icon} {r.GroupName}", value = r.Despesa })
            .ToList();

        return JsonSerializer.Serialize(new { labels = slices.Select(s => s.label), values = slices.Select(s => s.value) }, JsonOpts);
    }

    private static string BuildComparativoChartJson(IReadOnlyList<GroupedReportRowVm> rows)
    {
        var top = rows
            .Where(r => r.Receita > 0 || r.Despesa > 0)
            .OrderByDescending(r => r.Despesa + r.Receita)
            .Take(8)
            .ToList();

        return JsonSerializer.Serialize(new
        {
            labels = top.Select(r => r.GroupName),
            receitas = top.Select(r => r.Receita),
            despesas = top.Select(r => r.Despesa),
        }, JsonOpts);
    }

    private static string BuildDailyChartJson(IReadOnlyList<TransactionDto> noPeriodo, DateTime reference, FinancialFormatContext fmt)
    {
        var daysInMonth = DateTime.DaysInMonth(reference.Year, reference.Month);
        var labels = new List<string>();
        var receitas = new List<decimal>();
        var despesas = new List<decimal>();

        for (var day = 1; day <= daysInMonth; day++)
        {
            labels.Add(day.ToString("00", fmt.Culture));
            var dayStart = new DateTime(reference.Year, reference.Month, day, 0, 0, 0, DateTimeKind.Utc);
            var dayEnd = dayStart.AddDays(1);

            decimal rec = 0, desp = 0;
            foreach (var t in noPeriodo)
            {
                var d = t.Date.UtcDateTime;
                if (d < dayStart || d >= dayEnd) continue;
                if (t.TransactionTypeKind == TransactionTypeKind.Receita)
                    rec += t.TransactionValue;
                else if (t.TransactionTypeKind == TransactionTypeKind.Despesa)
                    desp += t.TransactionValue;
            }

            receitas.Add(rec);
            despesas.Add(desp);
        }

        return JsonSerializer.Serialize(new { labels, receitas, despesas }, JsonOpts);
    }
}
