namespace FinanceControl.Contracts.Filters;

/// <summary>Filtro genérico para listagens paginadas (espelho Seven.Support.V3.Contracts).</summary>
public class DataFilterDto
{
    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;

    public Dictionary<string, string>? Filters { get; set; }
}
