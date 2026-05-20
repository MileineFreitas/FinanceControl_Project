namespace FinanceControl.Contracts.Constants;

/// <summary>Ícones permitidos no cadastro de categorias (emoji).</summary>
public static class CategoryIcons
{
    public const string Default = "📁";

    public static readonly IReadOnlyList<string> Available =
    [
        "🛒", "🚀", "🏋️", "🏥", "🎓", "🐶",
        "✈️", "💎", "🐷", "🎁", "🚲", "🔧",
        "🍔", "🚗", "🏠", "💊", "📚", "🎮",
        "💰", "💼", "📈", "🛍️", "☕", "🎬"
    ];

    public static string Normalize(string? icon)
    {
        if (string.IsNullOrWhiteSpace(icon)) return Default;
        var trimmed = icon.Trim();
        return Available.Contains(trimmed) ? trimmed : Default;
    }
}
