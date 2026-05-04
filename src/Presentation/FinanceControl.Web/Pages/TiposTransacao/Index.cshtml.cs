using System.Text.Json;
using FinanceControl.Web.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FinanceControl.Web.Pages.TiposTransacao;

public class IndexModel : PageModel
{
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    private readonly IFinanceControlApiClient _api;

    public IndexModel(IFinanceControlApiClient api) => _api = api;

    public List<TipoTransacaoVm> Tipos { get; set; } = [];

    public string? ApiMensagem { get; set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        try
        {
            var res = await _api.GetTransactionTypesAsync(cancellationToken);
            if (!res.IsSuccessStatusCode)
            {
                ApiMensagem = "API indisponível — mostrando tipos padrão.";
                Tipos = TipoTransacaoVm.Padrao;
                return;
            }

            await using var stream = await res.Content.ReadAsStreamAsync(cancellationToken);
            var arr = await JsonSerializer.DeserializeAsync<List<TipoJson>>(stream, JsonOpts, cancellationToken);
            if (arr is { Count: > 0 })
            {
                Tipos = arr.Select(t => new TipoTransacaoVm(t.TransactionTypeId, t.Name ?? "", "Ativo")).ToList();
                return;
            }
        }
        catch
        {
            ApiMensagem = "Não foi possível carregar os tipos.";
        }

        Tipos = TipoTransacaoVm.Padrao;
    }

    private sealed class TipoJson
    {
        public int TransactionTypeId { get; set; }
        public string? Name { get; set; }
    }
}

public sealed record TipoTransacaoVm(int Id, string Nome, string Status)
{
    public static readonly List<TipoTransacaoVm> Padrao =
    [
        new(1, "RECEITA", "Ativo"),
        new(2, "DESPESA", "Ativo"),
    ];
}
