namespace FinanceControl.Web.Options;

public sealed class ApiClientOptions
{
    public const string SectionName = "ApiClient";

    /// <summary>Base URL da FinanceControl API (sem barra final).</summary>
    public string BaseUrl { get; set; } = "https://localhost:7189";

    public int TimeoutSeconds { get; set; } = 100;
}
