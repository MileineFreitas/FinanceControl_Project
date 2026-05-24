using FinanceControl.Contracts.Enumerators.Transactions;

namespace FinanceControl.Web.Models.ViewModels.Transactions;

public sealed class TransactionIndexViewModel
{
    public List<TransacaoListaVm> Transacoes { get; set; } = [];

    public List<CategoriaOpcaoVm> CategoriasOpcoes { get; set; } = [];

    public List<MeioPagamentoOpcaoVm> MeiosPagamentoOpcoes { get; set; } = [];

    public string? ApiMensagem { get; set; }

    public string? FiltroTipo { get; set; }

    public Guid? FiltroCategoriaId { get; set; }

    public string? Busca { get; set; }

    public int Pag { get; set; } = 1;

    public string OrdenarPor { get; set; } = "data";

    public string Ordem { get; set; } = "desc";

    public TransacaoFormInput Input { get; set; } = new();

    public Guid? EditingId { get; set; }

    public Guid AccountIdEdicao { get; set; }

    public TransactionStatus StatusEdicao { get; set; } = TransactionStatus.Pago;

    public bool ModalAberto { get; set; }

    public bool ModoEdicao => EditingId is not null && EditingId != Guid.Empty;

    public string? ErroModal { get; set; }

    public int TamanhoPagina { get; } = 10;

    public int TotalItens { get; set; }

    public int TotalPaginas => Math.Max(1, (int)Math.Ceiling(TotalItens / (double)TamanhoPagina));

    public string ResumoSaldoMensal { get; set; } = "R$ 0,00";

    public string ResumoMaiorGasto { get; set; } = "—";

    public string AlertaOrcamento { get; set; } = "Cadastre transações para acompanhar seu orçamento.";

    public Dictionary<string, string> RotasPagina(int? pag = null)
    {
        var rotas = new Dictionary<string, string>
        {
            ["OrdenarPor"] = OrdenarPor,
            ["Ordem"] = Ordem,
            ["Pag"] = (pag ?? Pag).ToString()
        };
        if (!string.IsNullOrEmpty(FiltroTipo)) rotas["FiltroTipo"] = FiltroTipo;
        if (FiltroCategoriaId is Guid cat && cat != Guid.Empty) rotas["FiltroCategoriaId"] = cat.ToString();
        if (!string.IsNullOrWhiteSpace(Busca)) rotas["Busca"] = Busca.Trim();
        return rotas;
    }

    public Dictionary<string, string> RotasOrdenacao(string coluna)
    {
        var rotas = RotasPagina(1);
        rotas["OrdenarPor"] = coluna;
        rotas["Ordem"] = ProximaOrdem(coluna);
        return rotas;
    }

    public bool ColunaOrdenada(string coluna) =>
        string.Equals(OrdenarPor, coluna, StringComparison.OrdinalIgnoreCase);

    public string IconeOrdenacao(string coluna)
    {
        if (!ColunaOrdenada(coluna)) return "↕";
        return string.Equals(Ordem, "asc", StringComparison.OrdinalIgnoreCase) ? "↑" : "↓";
    }

    private string ProximaOrdem(string coluna)
    {
        if (ColunaOrdenada(coluna))
            return string.Equals(Ordem, "desc", StringComparison.OrdinalIgnoreCase) ? "asc" : "desc";

        return coluna is "descricao" or "categoria" ? "asc" : "desc";
    }
}

public sealed class TransacaoFormInput
{
    public DateTime Data { get; set; } = DateTime.Today;

    public int TipoTransacao { get; set; } = (int)TransactionTypeKind.Receita;

    public Guid PaymentMethodId { get; set; }

    public string Descricao { get; set; } = "";

    public decimal Valor { get; set; }

    public Guid CategoryId { get; set; }
}

public sealed record TransacaoListaVm(
    Guid Id,
    DateTime Data,
    string Descricao,
    string CategoriaNome,
    Guid CategoriaId,
    TransactionTypeKind TransactionTypeKind,
    string? MeioPagamento,
    decimal Valor)
{
    public bool IsReceita => TransactionTypeKind == TransactionTypeKind.Receita;
}

public sealed record CategoriaOpcaoVm(Guid Id, string Nome, string Icone = "📁");

public sealed record MeioPagamentoOpcaoVm(Guid Id, string Nome, string Icone, PaymentKind? PaymentKind);
