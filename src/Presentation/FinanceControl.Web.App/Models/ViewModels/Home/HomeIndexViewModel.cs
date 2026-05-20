using FinanceControl.Web.Models.ViewModels;

namespace FinanceControl.Web.Models.ViewModels.Home;

public sealed class HomeIndexViewModel
{
    public IReadOnlyList<DashboardMetricVm> Metrics { get; set; } = [];

    public IReadOnlyList<DashboardTxRowVm> TransacoesResumo { get; set; } = [];

    public int TotalTransacoesExemplo { get; set; }

    public int TransacoesMostradas { get; set; }

    public string? ApiMensagem { get; set; }
}
