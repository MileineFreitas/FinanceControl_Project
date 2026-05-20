using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace FinanceControl.Contracts.Constants;

/// <summary>Gera código interno único a partir do nome do meio de pagamento.</summary>
public static partial class PaymentMethodCodes
{
    public static string FromName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return "MEIO";

        var normalized = name.Trim().Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder(normalized.Length);
        foreach (var c in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.NonSpacingMark)
                continue;
            sb.Append(c);
        }

        var code = NonAlphaNumeric().Replace(sb.ToString().ToUpperInvariant(), "_").Trim('_');
        while (code.Contains("__", StringComparison.Ordinal))
            code = code.Replace("__", "_", StringComparison.Ordinal);

        if (code.Length < 2)
            code = "MEIO";

        return code.Length > 20 ? code[..20].TrimEnd('_') : code;
    }

    [GeneratedRegex(@"[^A-Z0-9]+", RegexOptions.CultureInvariant)]
    private static partial Regex NonAlphaNumeric();
}
