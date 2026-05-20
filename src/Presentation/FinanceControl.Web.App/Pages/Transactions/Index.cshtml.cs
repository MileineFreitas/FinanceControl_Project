using System.Text.Json;
using FinanceControl.Domain.Entities;
using FinanceControl.Domain.Enums;
using FinanceControl.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FinanceControl.Web.Pages.Transactions;

public class IndexModel : PageModel
{
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    private readonly IFinanceControlApiClient _api;

    public IndexModel(IFinanceControlApiClient api) => _api = api;

    public List<TransacaoListaVm> Transacoes { get; set; } = [];

    public List<CategoriaOpcaoVm> CategoriasOpcoes { get; set; } = [];

    public string? ApiMensagem { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? FiltroTipo { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? FiltroCategoriaId { get; set; }

    [BindProperty(SupportsGet = true)]
    public int Pag { get; set; } = 1;

    [BindProperty]
    public TransacaoFormInput Input { get; set; } = new();

    public bool ModalAberto { get; set; }

    public string? ErroModal { get; set; }

    public int TamanhoPagina { get; } = 10;

    public int TotalItens { get; set; }

    public int TotalPaginas => Math.Max(1, (int)Math.Ceiling(TotalItens / (double)TamanhoPagina));

    public string ResumoSaldoMensal { get; set; } = "R$ 13.473,60";

    public string ResumoMaiorGasto { get; set; } = "Moradia";

    public string AlertaOrcamento { get; set; } = "Você está próximo de atingir o limite na categoria 'Lazer'. Recomenda-se cautela.";

    private List<TransacaoListaVm> _todas = [];

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        await CarregarCategoriasAsync(cancellationToken);
        await CarregarTransacoesAsync(cancellationToken);
        AplicarFiltrosEPaginar();
    }

    private void AplicarFiltrosEPaginar()
    {
        IEnumerable<TransacaoListaVm> q = _todas;
        if (FiltroTipo == "1" || FiltroTipo == "2")
            q = q.Where(t => t.TipoId == int.Parse(FiltroTipo));
        if (FiltroCategoriaId is int categoriaFiltro && categoriaFiltro > 0)
            q = q.Where(t => t.CategoriaId == categoriaFiltro);

        var list = q.OrderByDescending(t => t.Data).ToList();
        TotalItens = list.Count;
        if (Pag < 1) Pag = 1;
        if (Pag > TotalPaginas && TotalPaginas > 0) Pag = TotalPaginas;
        Transacoes = list.Skip((Pag - 1) * TamanhoPagina).Take(TamanhoPagina).ToList();
    }

    private async Task CarregarCategoriasAsync(CancellationToken cancellationToken)
    {
        CategoriasOpcoes.Clear();
        try
        {
            var res = await _api.GetCategoriesAsync(cancellationToken);
            if (!res.IsSuccessStatusCode)
            {
                ApiMensagem ??= "Categorias: API indisponível. Não é possível escolher categoria até a API responder.";
                return;
            }

            await using var stream = await res.Content.ReadAsStreamAsync(cancellationToken);
            var arr = await JsonSerializer.DeserializeAsync<List<CategoryJson>>(stream, JsonOpts, cancellationToken);
            if (arr is { Count: > 0 })
            {
                foreach (var c in arr)
                    CategoriasOpcoes.Add(new CategoriaOpcaoVm(c.CategoryId, c.CategoryName ?? "", c.TransactionTypeId ?? 1));
                return;
            }

            ApiMensagem ??= "Nenhuma categoria na base. Cadastre em Categorias ou reinicie a API (seed cria categorias padrão se a tabela estiver vazia).";
        }
        catch
        {
            ApiMensagem ??= "Não foi possível carregar categorias da API.";
        }
    }

    private async Task CarregarTransacoesAsync(CancellationToken cancellationToken)
    {
        _todas = [];
        try
        {
            var res = await _api.GetTransactionsAsync(cancellationToken);
            if (res.IsSuccessStatusCode)
            {
                await using var stream = await res.Content.ReadAsStreamAsync(cancellationToken);
                var arr = await JsonSerializer.DeserializeAsync<List<TransactionJson>>(stream, JsonOpts, cancellationToken);
                if (arr is { Count: > 0 })
                {
                    var catMap = CategoriasOpcoes.ToDictionary(c => c.Id, c => c.Nome);
                    foreach (var t in arr.OrderByDescending(x => x.Date))
                    {
                        var nome = catMap.TryGetValue(t.CategoryId, out var n) ? n : $"Cat #{t.CategoryId}";
                        _todas.Add(new TransacaoListaVm(
                            t.TransactionId,
                            t.Date,
                            t.TransactionDescription ?? "",
                            nome,
                            t.CategoryId,
                            t.TransactionTypeId,
                            t.TransactionValue));
                    }

                    return;
                }
            }
        }
        catch
        {
            ApiMensagem ??= "Transações: exibindo dados de exemplo (API indisponível).";
        }

        _todas = TransacaoListaVm.SampleData();
    }

    public async Task<IActionResult> OnPostNovaTransacaoAsync(CancellationToken cancellationToken)
    {
        ModalAberto = true;
        await CarregarCategoriasAsync(cancellationToken);
        await CarregarTransacoesAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(Input.Descricao) || Input.Descricao.Length < 2)
        {
            ErroModal = "Informe uma descrição (mín. 2 caracteres).";
            AplicarFiltrosEPaginar();
            return Page();
        }

        if (Input.Valor <= 0)
        {
            ErroModal = "Valor deve ser maior que zero.";
            AplicarFiltrosEPaginar();
            return Page();
        }

        if (Input.CategoryId <= 0)
        {
            ErroModal = "Selecione uma categoria.";
            AplicarFiltrosEPaginar();
            return Page();
        }

        var dto = new TransactionCreateDto
        {
            TransactionDescription = Input.Descricao.Trim(),
            TransactionValue = Input.Valor,
            Date = Input.Data.ToUniversalTime(),
            TransactionTypeId = Input.TipoFluxo is >= 1 and <= 2 ? Input.TipoFluxo : 1,
            CategoryId = Input.CategoryId,
            AccountId = 1,
            UserId = 1,
            Status = TransactionStatus.Pago
        };

        try
        {
            var response = await _api.CreateTransactionAsync(dto, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync(cancellationToken);
                ErroModal = $"API: {(int)response.StatusCode} — {body}";
                AplicarFiltrosEPaginar();
                return Page();
            }

            ModalAberto = false;
            ErroModal = null;
            Input = new TransacaoFormInput();
            await CarregarTransacoesAsync(cancellationToken);
            AplicarFiltrosEPaginar();
            return RedirectToPage(new { FiltroTipo, FiltroCategoriaId, Pag });
        }
        catch (Exception ex)
        {
            ErroModal = ex.Message;
            AplicarFiltrosEPaginar();
            return Page();
        }
    }

    public async Task<IActionResult> OnPostExcluirTransacaoAsync(int id, CancellationToken cancellationToken)
    {
        if (id <= 0)
            return RedirectToPage(new { FiltroTipo, FiltroCategoriaId, Pag });

        try
        {
            var res = await _api.DeleteTransactionAsync(id, cancellationToken);
            if (!res.IsSuccessStatusCode)
            {
                ApiMensagem = $"Não foi possível excluir a transação ({(int)res.StatusCode}).";
            }
        }
        catch (Exception ex)
        {
            ApiMensagem = ex.Message;
        }

        await CarregarTransacoesAsync(cancellationToken);
        AplicarFiltrosEPaginar();
        return RedirectToPage(new { FiltroTipo, FiltroCategoriaId, Pag });
    }

    private sealed class CategoryJson
    {
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public int? TransactionTypeId { get; set; }
    }

    private sealed class TransactionJson
    {
        public int TransactionId { get; set; }
        public string? TransactionDescription { get; set; }
        public decimal TransactionValue { get; set; }
        public DateTime Date { get; set; }
        public int TransactionTypeId { get; set; }
        public int CategoryId { get; set; }
    }
}

public sealed class TransacaoFormInput
{
    public DateTime Data { get; set; } = DateTime.Today;

    /// <summary>1 = receita (entrada), 2 = despesa (saída).</summary>
    public int TipoFluxo { get; set; } = 1;

    public string Descricao { get; set; } = "";

    public decimal Valor { get; set; }

    public int CategoryId { get; set; }
}

public sealed record TransacaoListaVm(
    int Id,
    DateTime Data,
    string Descricao,
    string CategoriaNome,
    int CategoriaId,
    int TipoId,
    decimal Valor)
{
    public bool IsReceita => TipoId == 1;

    public static List<TransacaoListaVm> SampleData() =>
    [
        new(0, new DateTime(2024, 1, 24, 0, 0, 0, DateTimeKind.Utc), "Dividendos - AAPL", "INVESTIMENTOS", 1, 1, 12500.00m),
        new(0, new DateTime(2024, 1, 22, 0, 0, 0, DateTimeKind.Utc), "Supermercado", "ALIMENTAÇÃO", 1, 2, -450.00m),
        new(0, new DateTime(2024, 1, 20, 0, 0, 0, DateTimeKind.Utc), "Netflix", "LAZER", 1, 2, -55.90m),
        new(0, new DateTime(2024, 1, 18, 0, 0, 0, DateTimeKind.Utc), "Salário", "SALÁRIO", 1, 1, 8500.00m),
        new(0, new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc), "Combustível", "TRANSPORTE", 1, 2, -280.00m),
        new(0, new DateTime(2024, 1, 12, 0, 0, 0, DateTimeKind.Utc), "Farmácia", "SAÚDE", 1, 2, -120.50m),
        new(0, new DateTime(2024, 1, 10, 0, 0, 0, DateTimeKind.Utc), "Freelance Design", "EXTRA", 1, 1, 3200.00m),
        new(0, new DateTime(2024, 1, 8, 0, 0, 0, DateTimeKind.Utc), "Aluguel", "MORADIA", 1, 2, -2200.00m),
        new(0, new DateTime(2024, 1, 5, 0, 0, 0, DateTimeKind.Utc), "Academia", "SAÚDE", 1, 2, -89.90m),
        new(0, new DateTime(2024, 1, 3, 0, 0, 0, DateTimeKind.Utc), "Transferência PIX", "OUTROS", 1, 1, 500.00m),
        new(0, new DateTime(2024, 1, 2, 0, 0, 0, DateTimeKind.Utc), "Restaurante", "LAZER", 1, 2, -180.00m),
        new(0, new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), "Ano novo — exemplo", "LAZER", 1, 2, -99.00m),
    ];
}

public sealed record CategoriaOpcaoVm(int Id, string Nome, int TransactionTypeId);
