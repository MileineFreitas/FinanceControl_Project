using System.Globalization;
using System.Security.Claims;
using FinanceControl.Contracts.Dtos.Users;

namespace FinanceControl.Web.Helpers;

public sealed class FinancialFormatContext
{
    public string Moeda { get; init; } = FinancialPreferenceDefaults.Moeda;

    public string Idioma { get; init; } = FinancialPreferenceDefaults.Idioma;

    public string FormatoData { get; init; } = FinancialPreferenceDefaults.FormatoData;

    public int InicioMes { get; init; } = FinancialPreferenceDefaults.InicioMes;

    public CultureInfo Culture => FinancialFormatHelper.GetCulture(Idioma, Moeda);

    public string FormatCurrency(decimal value) =>
        FinancialFormatHelper.FormatCurrency(value, Idioma, Moeda);

    public string FormatSignedCurrency(decimal value, bool isReceita) =>
        (isReceita ? "+ " : "- ") + FormatCurrency(Math.Abs(value));

    public string FormatDate(DateTime date) =>
        date.ToString(FormatoData, Culture);

    public string FormatDateTime(DateTime date) =>
        date.ToString($"{FormatoData} HH:mm", Culture);

    public string FormatDateTimeLong(DateTime date) =>
        date.ToString($"d MMM, yyyy HH:mm", Culture);

    public (DateTime Start, DateTime End) GetFinancialMonthRange(DateTime reference) =>
        FinancialFormatHelper.GetFinancialMonthRange(reference, InicioMes);

    public static FinancialFormatContext From(UserFinancialPreferencesDto dto) =>
        new()
        {
            Moeda = dto.Moeda,
            Idioma = dto.Idioma,
            FormatoData = dto.FormatoData,
            InicioMes = dto.InicioMes
        };

    public static FinancialFormatContext From(ClaimsPrincipal user) =>
        From(user.GetFinancialPreferences());

    public UserFinancialPreferencesDto ToDto() =>
        new()
        {
            Moeda = Moeda,
            Idioma = Idioma,
            FormatoData = FormatoData,
            InicioMes = InicioMes
        };
}

public static class FinancialFormatHelper
{
    public static CultureInfo GetCulture(string idioma, string moeda)
    {
        var culture = (CultureInfo)CultureInfo.GetCultureInfo(idioma).Clone();
        culture.NumberFormat.CurrencySymbol = moeda switch
        {
            "USD" => "US$",
            "EUR" => "€",
            _ => "R$"
        };
        return culture;
    }

    public static string FormatCurrency(decimal value, string idioma, string moeda) =>
        value.ToString("C", GetCulture(idioma, moeda));

    public static (DateTime Start, DateTime End) GetFinancialMonthRange(DateTime reference, int startDay)
    {
        var utc = reference.Kind == DateTimeKind.Utc
            ? reference
            : DateTime.SpecifyKind(reference, DateTimeKind.Utc);

        if (startDay <= 1)
        {
            var start = new DateTime(utc.Year, utc.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            return (start, start.AddMonths(1));
        }

        var year = utc.Year;
        var month = utc.Month;

        if (utc.Day < startDay)
        {
            var previousMonth = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(-1);
            var startDayActual = Math.Min(startDay, DateTime.DaysInMonth(previousMonth.Year, previousMonth.Month));
            var start = new DateTime(previousMonth.Year, previousMonth.Month, startDayActual, 0, 0, 0, DateTimeKind.Utc);
            var endDayActual = Math.Min(startDay, DateTime.DaysInMonth(year, month));
            var end = new DateTime(year, month, endDayActual, 0, 0, 0, DateTimeKind.Utc);
            return (start, end);
        }

        var currentStartDay = Math.Min(startDay, DateTime.DaysInMonth(year, month));
        var currentStart = new DateTime(year, month, currentStartDay, 0, 0, 0, DateTimeKind.Utc);
        var nextMonth = currentStart.AddMonths(1);
        var nextEndDay = Math.Min(startDay, DateTime.DaysInMonth(nextMonth.Year, nextMonth.Month));
        var currentEnd = new DateTime(nextMonth.Year, nextMonth.Month, nextEndDay, 0, 0, 0, DateTimeKind.Utc);
        return (currentStart, currentEnd);
    }
}
