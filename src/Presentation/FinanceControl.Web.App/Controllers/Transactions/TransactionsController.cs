using FinanceControl.Client.Services.Interfaces.Categories;
using FinanceControl.Client.Services.Interfaces.PaymentMethods;
using FinanceControl.Client.Services.Interfaces.Transactions;
using FinanceControl.Contracts.Constants;
using FinanceControl.Contracts.Dtos.Transactions;
using FinanceControl.Contracts.Enumerators.Transactions;
using FinanceControl.Contracts.Filters;
using FinanceControl.Web.Helpers;
using FinanceControl.Web.Models.ViewModels.Transactions;
using FinanceControl.Web.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Globalization;

namespace FinanceControl.Web.Controllers.Transactions;

[Route("transacoes")]
public class TransactionsController : Controller
{
    private readonly ITransactionCliService _transactionCli;
    private readonly ICategoryCliService _categoryCli;
    private readonly IPaymentMethodCliService _paymentMethodCli;
    private readonly IStringLocalizer<SharedResources> _localizer;

    private List<TransacaoListaVm> _todas = [];

    public TransactionsController(
        ITransactionCliService transactionCli,
        ICategoryCliService categoryCli,
        IPaymentMethodCliService paymentMethodCli,
        IStringLocalizer<SharedResources> localizer)
    {
        _transactionCli = transactionCli;
        _categoryCli = categoryCli;
        _paymentMethodCli = paymentMethodCli;
        _localizer = localizer;
    }

    [HttpGet("")]
    [HttpGet("Index")]
    public async Task<IActionResult> Index(TransactionIndexViewModel vm)
    {
        await CarregarCategoriasAsync(vm);
        await CarregarMeiosPagamentoAsync(vm);
        await CarregarTransacoesAsync(vm);
        AplicarFiltrosEPaginar(vm);
        return View("Index", vm);
    }

    [HttpGet("~/transactions")]
    public IActionResult TransactionsRedirect() =>
        RedirectToActionPermanent(nameof(Index));

    [HttpGet("Editar/{id:guid}")]
    public async Task<IActionResult> Editar(Guid id, TransactionIndexViewModel vm)
    {
        if (id == Guid.Empty)
            return RedirectToAction(nameof(Index), vm.RotasPagina());

        vm.EditingId = id;
        vm.ModalAberto = true;

        try
        {
            var dto = await _transactionCli.GetByIdAsync(id);
            if (dto == null)
            {
                vm.ApiMensagem = "Transação não encontrada.";
                vm.EditingId = null;
                vm.ModalAberto = false;
                await CarregarCategoriasAsync(vm);
                await CarregarMeiosPagamentoAsync(vm);
                await CarregarTransacoesAsync(vm);
                AplicarFiltrosEPaginar(vm);
                return View("Index", vm);
            }

            await CarregarCategoriasAsync(vm, dto.CategoryId);
            await CarregarMeiosPagamentoAsync(vm, dto.PaymentMethodId);
            await CarregarTransacoesAsync(vm);
            PreencherFormularioEdicao(vm, dto);
        }
        catch (Exception ex)
        {
            vm.ApiMensagem = $"Não foi possível carregar a transação: {ex.Message}";
            vm.EditingId = null;
            vm.ModalAberto = false;
            await CarregarCategoriasAsync(vm);
            await CarregarMeiosPagamentoAsync(vm);
            await CarregarTransacoesAsync(vm);
        }

        AplicarFiltrosEPaginar(vm);
        return View("Index", vm);
    }

    [HttpPost("Salvar")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Salvar(TransactionIndexViewModel vm)
    {
        var userId = User.GetUserId();
        if (userId == null)
            return RedirectToAction("Index", "Login");

        vm.ModalAberto = true;
        var categoriaIncluir = vm.Input.CategoryId != Guid.Empty ? vm.Input.CategoryId : (Guid?)null;
        var meioIncluir = vm.Input.PaymentMethodId != Guid.Empty ? vm.Input.PaymentMethodId : (Guid?)null;
        await CarregarCategoriasAsync(vm, categoriaIncluir);
        await CarregarMeiosPagamentoAsync(vm, meioIncluir);
        await CarregarTransacoesAsync(vm);

        if (!ValidarFormulario(vm, out var tipo))
        {
            AplicarFiltrosEPaginar(vm);
            return View("Index", vm);
        }

        var dataUtc = new DateTimeOffset(DateTime.SpecifyKind(vm.Input.Data.Date, DateTimeKind.Utc));

        try
        {
            HttpResponseMessage response;
            if (vm.EditingId is Guid editId && editId != Guid.Empty)
            {
                if (vm.AccountIdEdicao == Guid.Empty)
                {
                    vm.ErroModal = "Conta da transação não disponível.";
                    AplicarFiltrosEPaginar(vm);
                    return View("Index", vm);
                }

                var update = new TransactionUpdateDto
                {
                    TransactionId = editId,
                    TransactionDescription = vm.Input.Descricao.Trim(),
                    TransactionValue = vm.Input.Valor,
                    Date = dataUtc,
                    TransactionTypeKind = tipo,
                    PaymentMethodId = vm.Input.PaymentMethodId,
                    CategoryId = vm.Input.CategoryId,
                    AccountId = vm.AccountIdEdicao
                };
                response = await _transactionCli.UpdateAsync(editId, update);
            }
            else
            {
                var create = new TransactionCreateDto
                {
                    TransactionDescription = vm.Input.Descricao.Trim(),
                    TransactionValue = vm.Input.Valor,
                    Date = dataUtc,
                    TransactionTypeKind = tipo,
                    PaymentMethodId = vm.Input.PaymentMethodId,
                    CategoryId = vm.Input.CategoryId,
                    UserId = userId.Value
                };
                response = await _transactionCli.CreateAsync(create);
            }

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                vm.ErroModal = string.IsNullOrWhiteSpace(body)
                    ? $"Erro ao salvar ({(int)response.StatusCode})."
                    : body;
                AplicarFiltrosEPaginar(vm);
                return View("Index", vm);
            }

            return RedirectToAction(nameof(Index), vm.RotasPagina());
        }
        catch (Exception ex)
        {
            vm.ErroModal = ex.Message;
            AplicarFiltrosEPaginar(vm);
            return View("Index", vm);
        }
    }

    [HttpPost("Excluir/{id:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Excluir(Guid id, TransactionIndexViewModel vm)
    {
        if (id == Guid.Empty)
            return RedirectToAction(nameof(Index), vm.RotasPagina());

        try
        {
            var res = await _transactionCli.DeleteAsync(id);
            if (!res.IsSuccessStatusCode)
                vm.ApiMensagem = $"Não foi possível excluir a transação ({(int)res.StatusCode}).";
        }
        catch (Exception ex)
        {
            vm.ApiMensagem = ex.Message;
        }

        return RedirectToAction(nameof(Index), vm.RotasPagina());
    }

    private void AplicarFiltrosEPaginar(TransactionIndexViewModel vm)
    {
        IEnumerable<TransacaoListaVm> q = _todas;
        if (vm.FiltroTipo == "1" || vm.FiltroTipo == "2")
            q = q.Where(t => (int)t.TransactionTypeKind == int.Parse(vm.FiltroTipo));
        if (vm.FiltroCategoriaId is Guid categoriaFiltro && categoriaFiltro != Guid.Empty)
            q = q.Where(t => t.CategoriaId == categoriaFiltro);

        if (!string.IsNullOrWhiteSpace(vm.Busca))
        {
            var termo = vm.Busca.Trim();
            q = q.Where(t =>
                t.Descricao.Contains(termo, StringComparison.OrdinalIgnoreCase)
                || t.CategoriaNome.Contains(termo, StringComparison.OrdinalIgnoreCase)
                || (t.MeioPagamento?.Contains(termo, StringComparison.OrdinalIgnoreCase) ?? false));
        }

        var list = AplicarOrdenacao(vm, q).ToList();
        AtualizarResumos(vm, list);
        vm.TotalItens = list.Count;
        if (vm.Pag < 1) vm.Pag = 1;
        if (vm.Pag > vm.TotalPaginas && vm.TotalPaginas > 0) vm.Pag = vm.TotalPaginas;
        vm.Transacoes = list.Skip((vm.Pag - 1) * vm.TamanhoPagina).Take(vm.TamanhoPagina).ToList();
    }

    private async Task CarregarCategoriasAsync(TransactionIndexViewModel vm, Guid? incluirCategoriaId = null)
    {
        vm.CategoriasOpcoes.Clear();
        try
        {
            var data = await _categoryCli.ListAsync(
                new DataFilterDto { Page = 1, PageSize = 200 },
                includeInactive: false);

            if (data?.Result is not { Count: > 0 })
            {
                if (incluirCategoriaId is Guid incluirId && incluirId != Guid.Empty)
                {
                    await IncluirCategoriaOpcionalAsync(vm, incluirId);
                }

                if (vm.CategoriasOpcoes.Count == 0)
                    vm.ApiMensagem ??= "Nenhuma categoria ativa cadastrada.";
                return;
            }

            foreach (var c in data.Result
                         .Where(c => c.IsActive)
                         .OrderBy(c => c.CategoryName))
            {
                vm.CategoriasOpcoes.Add(new CategoriaOpcaoVm(
                    c.CategoryId,
                    c.CategoryName ?? "—",
                    CategoryIcons.Normalize(c.Icon)));
            }

            if (incluirCategoriaId is Guid categoriaId && categoriaId != Guid.Empty)
                await IncluirCategoriaOpcionalAsync(vm, categoriaId);
        }
        catch (Exception ex)
        {
            vm.ApiMensagem ??= $"Não foi possível carregar categorias: {ex.Message}";
        }
    }

    private async Task IncluirCategoriaOpcionalAsync(TransactionIndexViewModel vm, Guid categoriaId)
    {
        if (vm.CategoriasOpcoes.Any(c => c.Id == categoriaId))
            return;

        var categoria = await _categoryCli.GetByIdAsync(categoriaId);
        if (categoria == null)
            return;

        vm.CategoriasOpcoes.Add(new CategoriaOpcaoVm(
            categoria.CategoryId,
            categoria.CategoryName ?? "—",
            CategoryIcons.Normalize(categoria.Icon)));
    }

    private async Task CarregarMeiosPagamentoAsync(TransactionIndexViewModel vm, Guid? incluirMeioId = null)
    {
        vm.MeiosPagamentoOpcoes.Clear();
        try
        {
            var list = await _paymentMethodCli.ListAsync(includeInactive: false);
            if (list is { Count: > 0 })
            {
                foreach (var m in list.Where(t => t.IsActive).OrderBy(t => t.Name))
                {
                    vm.MeiosPagamentoOpcoes.Add(new MeioPagamentoOpcaoVm(
                        m.PaymentMethodId,
                        m.Name,
                        PaymentMethodIcons.Normalize(m.Icon)));
                }
            }

            if (incluirMeioId is Guid meioId && meioId != Guid.Empty)
                await IncluirMeioPagamentoOpcionalAsync(vm, meioId);
        }
        catch (Exception ex)
        {
            vm.ApiMensagem ??= $"Não foi possível carregar meios de pagamento: {ex.Message}";
        }
    }

    private async Task IncluirMeioPagamentoOpcionalAsync(TransactionIndexViewModel vm, Guid meioId)
    {
        if (vm.MeiosPagamentoOpcoes.Any(m => m.Id == meioId))
            return;

        var meio = await _paymentMethodCli.GetByIdAsync(meioId);
        if (meio == null)
            return;

        vm.MeiosPagamentoOpcoes.Add(new MeioPagamentoOpcaoVm(
            meio.PaymentMethodId,
            meio.Name,
            PaymentMethodIcons.Normalize(meio.Icon)));
    }

    private async Task CarregarTransacoesAsync(TransactionIndexViewModel vm)
    {
        _todas = [];
        try
        {
            var data = await _transactionCli.ListAsync(
                new DataFilterDto { Page = 1, PageSize = 500 });

            if (data?.Result is not { Count: > 0 })
            {
                if (data != null && data.Total == 0)
                    vm.ApiMensagem ??= "Nenhuma transação cadastrada ainda.";
                else if (data == null)
                    vm.ApiMensagem ??= "Não foi possível ler as transações da API.";
                return;
            }

            foreach (var t in data.Result)
            {
                _todas.Add(ToListaVm(vm, t));
            }

            if (data.Total > data.Result.Count)
            {
                vm.ApiMensagem ??=
                    $"Exibindo {data.Result.Count} de {data.Total} transações. Ajuste os filtros ou aumente o limite no servidor.";
            }
        }
        catch (Exception ex)
        {
            vm.ApiMensagem = $"Não foi possível carregar transações da API: {ex.Message}";
        }
    }

    private TransacaoListaVm ToListaVm(TransactionIndexViewModel vm, TransactionDto t)
    {
        var categoria = !string.IsNullOrWhiteSpace(t.CategoryName)
            ? t.CategoryName
            : vm.CategoriasOpcoes.FirstOrDefault(c => c.Id == t.CategoryId)?.Nome ?? $"Cat #{t.CategoryId}";

        return new TransacaoListaVm(
            t.TransactionId,
            t.Date.UtcDateTime,
            t.TransactionDescription ?? "",
            categoria,
            t.CategoryId,
            t.TransactionTypeKind,
            t.PaymentMethodName ?? vm.MeiosPagamentoOpcoes.FirstOrDefault(m => m.Id == t.PaymentMethodId)?.Nome,
            t.TransactionValue);
    }

    private void AtualizarResumos(TransactionIndexViewModel vm, IReadOnlyList<TransacaoListaVm> filtradas)
    {
        var fmt = FinancialFormatContext.From(User);
        var culture = fmt.Culture;

        if (filtradas.Count == 0)
        {
            vm.ResumoSaldoMensal = fmt.FormatCurrency(0);
            vm.ResumoSaldoMensalNegativo = false;
            vm.ResumoSaldoHint = "";
            vm.ResumoMaiorGasto = "—";
            vm.ResumoMaiorGastoHint = "";
            vm.AlertaOrcamento = _localizer["Messages.BudgetNoTx"].Value;
            return;
        }

        var saldo = filtradas.Sum(t => t.IsReceita ? t.Valor : -t.Valor);
        vm.ResumoSaldoMensal = fmt.FormatCurrency(saldo);
        vm.ResumoSaldoMensalNegativo = saldo < 0;
        vm.ResumoSaldoHint = MontarHintSaldo(filtradas, culture);

        var despesas = filtradas.Where(t => !t.IsReceita).ToList();
        var maiorDespesa = despesas.OrderByDescending(t => t.Valor).FirstOrDefault();

        if (maiorDespesa != null)
        {
            vm.ResumoMaiorGasto = fmt.FormatCurrency(maiorDespesa.Valor);
            var totalDespesas = despesas.Sum(t => t.Valor);
            var share = totalDespesas > 0 ? maiorDespesa.Valor / totalDespesas : 0m;
            var pctFormatted = share.ToString("P1", culture);
            var shareText = _localizer["Transactions.BudgetShare", pctFormatted].Value;
            vm.ResumoMaiorGastoHint = _localizer["Transactions.BiggestExpenseHint", maiorDespesa.CategoriaNome, shareText].Value;
        }
        else
        {
            vm.ResumoMaiorGasto = "—";
            vm.ResumoMaiorGastoHint = _localizer["Messages.BudgetNoExpense"].Value;
        }

        var categoriaMaisGasta = despesas
            .GroupBy(t => t.CategoriaNome)
            .OrderByDescending(g => g.Sum(t => t.Valor))
            .Select(g => g.Key)
            .FirstOrDefault();

        vm.AlertaOrcamento = categoriaMaisGasta != null
            ? _localizer["Messages.BudgetTopCategory", categoriaMaisGasta].Value
            : _localizer["Messages.BudgetNoExpense"].Value;
    }

    private string MontarHintSaldo(IReadOnlyList<TransacaoListaVm> filtradas, CultureInfo culture)
    {
        var fmt = FinancialFormatContext.From(User);
        var now = DateTime.UtcNow;
        var (inicioMes, fimMes) = fmt.GetFinancialMonthRange(now);
        var (inicioAnterior, fimAnterior) = fmt.GetFinancialMonthRange(inicioMes.AddDays(-1));

        var saldoMesAtual = SaldoNoPeriodo(filtradas, inicioMes, fimMes);
        var saldoMesAnterior = SaldoNoPeriodo(filtradas, inicioAnterior, fimAnterior);

        if (saldoMesAnterior == 0 && saldoMesAtual == 0)
            return _localizer["Transactions.NoBalanceChange"].Value;

        if (saldoMesAnterior == 0)
            return _localizer["Transactions.NoPreviousMonthBalance"].Value;

        var variacao = (saldoMesAtual - saldoMesAnterior) / Math.Abs(saldoMesAnterior);
        var pctFormatted = variacao.ToString("P1", culture);
        var trendKey = variacao >= 0 ? "Transactions.TrendUp" : "Transactions.TrendDown";
        return $"{_localizer[trendKey, pctFormatted].Value} {_localizer["Transactions.VsPreviousMonth"].Value}";
    }

    private static decimal SaldoNoPeriodo(IEnumerable<TransacaoListaVm> transacoes, DateTime inicio, DateTime fim) =>
        transacoes
            .Where(t => t.Data >= inicio && t.Data < fim)
            .Sum(t => t.IsReceita ? t.Valor : -t.Valor);

    private void PreencherFormularioEdicao(TransactionIndexViewModel vm, TransactionDto dto)
    {
        var dataLocal = dto.Date.UtcDateTime.Date;

        vm.Input = new TransacaoFormInput
        {
            Data = dataLocal,
            TipoTransacao = (int)dto.TransactionTypeKind,
            Descricao = dto.TransactionDescription ?? "",
            Valor = dto.TransactionValue,
            CategoryId = dto.CategoryId,
            PaymentMethodId = dto.PaymentMethodId
        };
        vm.AccountIdEdicao = dto.AccountId;
    }

    private bool ValidarFormulario(TransactionIndexViewModel vm, out TransactionTypeKind tipo)
    {
        tipo = TransactionTypeKind.Receita;

        if (string.IsNullOrWhiteSpace(vm.Input.Descricao) || vm.Input.Descricao.Length < 2)
        {
            vm.ErroModal = _localizer["Messages.TxDescriptionRequired"].Value;
            return false;
        }

        if (vm.Input.Valor <= 0)
        {
            vm.ErroModal = _localizer["Messages.TxValueRequired"].Value;
            return false;
        }

        if (vm.Input.CategoryId == Guid.Empty)
        {
            vm.ErroModal = _localizer["Messages.TxCategoryRequired"].Value;
            return false;
        }

        if (vm.CategoriasOpcoes.All(c => c.Id != vm.Input.CategoryId))
        {
            vm.ErroModal = _localizer["Messages.TxCategoryInvalid"].Value;
            return false;
        }

        tipo = vm.Input.TipoTransacao is (int)TransactionTypeKind.Receita or (int)TransactionTypeKind.Despesa
            ? (TransactionTypeKind)vm.Input.TipoTransacao
            : TransactionTypeKind.Receita;

        if (vm.Input.PaymentMethodId == Guid.Empty)
        {
            vm.ErroModal = _localizer["Messages.TxPaymentRequired"].Value;
            return false;
        }

        if (vm.MeiosPagamentoOpcoes.All(m => m.Id != vm.Input.PaymentMethodId))
        {
            vm.ErroModal = _localizer["Messages.TxPaymentInvalid"].Value;
            return false;
        }

        return true;
    }

    private IEnumerable<TransacaoListaVm> AplicarOrdenacao(
        TransactionIndexViewModel vm,
        IEnumerable<TransacaoListaVm> query)
    {
        var desc = !string.Equals(vm.Ordem, "asc", StringComparison.OrdinalIgnoreCase);
        return (vm.OrdenarPor?.ToLowerInvariant() ?? "data") switch
        {
            "descricao" => desc
                ? query.OrderByDescending(t => t.Descricao, StringComparer.OrdinalIgnoreCase)
                : query.OrderBy(t => t.Descricao, StringComparer.OrdinalIgnoreCase),
            "categoria" => desc
                ? query.OrderByDescending(t => t.CategoriaNome, StringComparer.OrdinalIgnoreCase)
                : query.OrderBy(t => t.CategoriaNome, StringComparer.OrdinalIgnoreCase),
            "valor" => desc
                ? query.OrderByDescending(t => ValorComSinal(t))
                : query.OrderBy(t => ValorComSinal(t)),
            _ => desc
                ? query.OrderByDescending(t => t.Data)
                : query.OrderBy(t => t.Data)
        };
    }

    private static decimal ValorComSinal(TransacaoListaVm t) =>
        t.IsReceita ? t.Valor : -t.Valor;
}
