using FinanceControl.Web.Models.ViewModels;

namespace FinanceControl.Web.Models.ViewModels.Home;

public sealed class HomeIndexViewModel
{
    public IReadOnlyList<DashboardMetricVm> Metrics { get; set; } = [];

    public IReadOnlyList<DashboardTxRowVm> TransacoesResumo { get; set; } = [];

    public int TotalTransacoes { get; set; }

    public int TransacoesMostradas { get; set; }

    public string? ApiMensagem { get; set; }

    public string Idioma { get; set; } = "pt-BR";

    public string Moeda { get; set; } = "BRL";
}
