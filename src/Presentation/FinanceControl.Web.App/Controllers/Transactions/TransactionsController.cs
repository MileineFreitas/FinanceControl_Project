using System.Globalization;
using FinanceControl.Client.Services.Interfaces.Accounts;
using FinanceControl.Client.Services.Interfaces.Categories;
using FinanceControl.Client.Services.Interfaces.Transactions;
using FinanceControl.Client.Services.Interfaces.TransactionTypes;
using FinanceControl.Contracts.Constants;
using FinanceControl.Contracts.Dtos.Transactions;
using FinanceControl.Contracts.Enumerators.Transactions;
using FinanceControl.Contracts.Filters;
using FinanceControl.Web.Models.ViewModels.Transactions;
using Microsoft.AspNetCore.Mvc;

namespace FinanceControl.Web.Controllers.Transactions;

[Route("transacoes")]
public class TransactionsController : Controller
{
    private static readonly CultureInfo PtBr = CultureInfo.GetCultureInfo("pt-BR");

    private readonly ITransactionCliService _transactionCli;
    private readonly ICategoryCliService _categoryCli;
    private readonly ITransactionTypeCliService _paymentMethodCli;
    private readonly IAccountCliService _accountCli;

    private List<TransacaoListaVm> _todas = [];
    private int? _contaPadraoId;
    private int? _usuarioPadraoId;

    public TransactionsController(
        ITransactionCliService transactionCli,
        ICategoryCliService categoryCli,
        ITransactionTypeCliService paymentMethodCli,
        IAccountCliService accountCli)
    {
        _transactionCli = transactionCli;
        _categoryCli = categoryCli;
        _paymentMethodCli = paymentMethodCli;
        _accountCli = accountCli;
    }

    [HttpGet("")]
    [HttpGet("Index")]
    public async Task<IActionResult> Index(TransactionIndexViewModel vm)
    {
        await CarregarContaPadraoAsync(vm);
        await CarregarCategoriasAsync(vm);
        await CarregarMeiosPagamentoAsync(vm);
        await CarregarTransacoesAsync(vm);
        AplicarFiltrosEPaginar(vm);
        return View("Index", vm);
    }

    [HttpGet("~/transactions")]
    public IActionResult TransactionsRedirect() =>
        RedirectToActionPermanent(nameof(Index));

    [HttpGet("Editar/{id:int}")]
    public async Task<IActionResult> Editar(int id, TransactionIndexViewModel vm)
    {
        if (id <= 0)
            return RedirectToAction(nameof(Index), vm.RotasPagina());

        vm.EditingId = id;
        vm.ModalAberto = true;

        await CarregarContaPadraoAsync(vm);
        await CarregarCategoriasAsync(vm);
        await CarregarMeiosPagamentoAsync(vm);
        await CarregarTransacoesAsync(vm);

        try
        {
            var dto = await _transactionCli.GetByIdAsync(id);
            if (dto == null)
            {
                vm.ApiMensagem = "Transação não encontrada.";
                vm.EditingId = null;
                vm.ModalAberto = false;
                AplicarFiltrosEPaginar(vm);
                return View("Index", vm);
            }

            PreencherFormularioEdicao(vm, dto);
        }
        catch (Exception ex)
        {
            vm.ApiMensagem = $"Não foi possível carregar a transação: {ex.Message}";
            vm.EditingId = null;
            vm.ModalAberto = false;
        }

        AplicarFiltrosEPaginar(vm);
        return View("Index", vm);
    }

    [HttpPost("Salvar")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Salvar(TransactionIndexViewModel vm)
    {
        vm.ModalAberto = true;
        await CarregarContaPadraoAsync(vm);
        await CarregarCategoriasAsync(vm);
        await CarregarMeiosPagamentoAsync(vm);
        await CarregarTransacoesAsync(vm);

        if (!ValidarFormulario(vm, out var tipo, out var meio))
        {
            AplicarFiltrosEPaginar(vm);
            return View("Index", vm);
        }

        var dataUtc = DateTime.SpecifyKind(vm.Input.Data.Date, DateTimeKind.Utc);

        try
        {
            HttpResponseMessage response;
            if (vm.EditingId is int editId && editId > 0)
            {
                var accountId = vm.AccountIdEdicao > 0 ? vm.AccountIdEdicao : _contaPadraoId ?? 0;
                if (accountId <= 0)
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
                    PaymentKind = meio,
                    CategoryId = vm.Input.CategoryId,
                    AccountId = accountId,
                    Status = vm.StatusEdicao
                };
                response = await _transactionCli.UpdateAsync(editId, update);
            }
            else
            {
                if (_contaPadraoId is not int accountId || _usuarioPadraoId is not int userId)
                {
                    vm.ErroModal = "Conta padrão não disponível. Verifique se a API está em execução.";
                    AplicarFiltrosEPaginar(vm);
                    return View("Index", vm);
                }

                var create = new TransactionCreateDto
                {
                    TransactionDescription = vm.Input.Descricao.Trim(),
                    TransactionValue = vm.Input.Valor,
                    Date = dataUtc,
                    TransactionTypeKind = tipo,
                    PaymentKind = meio,
                    CategoryId = vm.Input.CategoryId,
                    AccountId = accountId,
                    UserId = userId,
                    Status = TransactionStatus.Pago
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

    [HttpPost("Excluir/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Excluir(int id, TransactionIndexViewModel vm)
    {
        if (id <= 0)
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
        if (vm.FiltroCategoriaId is int categoriaFiltro && categoriaFiltro > 0)
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

    private async Task CarregarContaPadraoAsync(TransactionIndexViewModel vm)
    {
        _contaPadraoId = null;
        _usuarioPadraoId = null;
        try
        {
            var contas = await _accountCli.ListAsync();
            var conta = contas?.OrderBy(c => c.AccountId).FirstOrDefault();
            if (conta == null)
            {
                vm.ApiMensagem ??= "Nenhuma conta encontrada. Verifique se a API e o seed foram executados.";
                return;
            }

            _contaPadraoId = conta.AccountId;
            _usuarioPadraoId = conta.UserId ?? 1;
        }
        catch (Exception ex)
        {
            vm.ApiMensagem ??= $"Não foi possível carregar a conta padrão: {ex.Message}";
        }
    }

    private async Task CarregarCategoriasAsync(TransactionIndexViewModel vm)
    {
        vm.CategoriasOpcoes.Clear();
        try
        {
            var data = await _categoryCli.ListAsync(
                new DataFilterDto { Page = 1, PageSize = 200 });

            if (data?.Result is { Count: > 0 })
            {
                foreach (var c in data.Result.OrderBy(c => c.CategoryName))
                {
                    vm.CategoriasOpcoes.Add(new CategoriaOpcaoVm(
                        c.CategoryId,
                        c.CategoryName ?? "—",
                        CategoryIcons.Normalize(c.Icon)));
                }
                return;
            }

            vm.ApiMensagem ??= "Nenhuma categoria cadastrada.";
        }
        catch (Exception ex)
        {
            vm.ApiMensagem ??= $"Não foi possível carregar categorias: {ex.Message}";
        }
    }

    private async Task CarregarMeiosPagamentoAsync(TransactionIndexViewModel vm)
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
                        m.TransactionTypeId,
                        m.Name,
                        PaymentMethodIcons.Normalize(m.Icon),
                        m.PaymentKind));
                }
            }
        }
        catch (Exception ex)
        {
            vm.ApiMensagem ??= $"Não foi possível carregar meios de pagamento: {ex.Message}";
        }
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
            t.Date,
            t.TransactionDescription ?? "",
            categoria,
            t.CategoryId,
            t.TransactionTypeKind,
            ResolverNomeMeioPagamento(vm, t.PaymentKind),
            t.TransactionValue);
    }

    private string? ResolverNomeMeioPagamento(TransactionIndexViewModel vm, PaymentKind? paymentKind)
    {
        if (paymentKind is null) return null;
        var match = vm.MeiosPagamentoOpcoes.FirstOrDefault(m => m.PaymentKind == paymentKind);
        return match?.Nome ?? paymentKind switch
        {
            PaymentKind.Debit => "Débito",
            PaymentKind.Credit => "Crédito",
            PaymentKind.Cash => "Dinheiro",
            _ => paymentKind.ToString()
        };
    }

    private void AtualizarResumos(TransactionIndexViewModel vm, IReadOnlyList<TransacaoListaVm> filtradas)
    {
        if (filtradas.Count == 0)
        {
            vm.ResumoSaldoMensal = "R$ 0,00";
            vm.ResumoMaiorGasto = "—";
            vm.AlertaOrcamento = "Nenhuma transação nos filtros atuais.";
            return;
        }

        var saldo = filtradas.Sum(t => t.IsReceita ? t.Valor : -t.Valor);
        vm.ResumoSaldoMensal = saldo.ToString("C", PtBr);

        var maiorDespesa = filtradas
            .Where(t => !t.IsReceita)
            .OrderByDescending(t => t.Valor)
            .FirstOrDefault();

        vm.ResumoMaiorGasto = maiorDespesa?.CategoriaNome ?? "—";

        var categoriaMaisGasta = filtradas
            .Where(t => !t.IsReceita)
            .GroupBy(t => t.CategoriaNome)
            .OrderByDescending(g => g.Sum(t => t.Valor))
            .Select(g => g.Key)
            .FirstOrDefault();

        vm.AlertaOrcamento = categoriaMaisGasta != null
            ? $"Maior volume de despesas na categoria «{categoriaMaisGasta}» (filtros atuais)."
            : "Nenhuma despesa nos filtros atuais.";
    }

    private void PreencherFormularioEdicao(TransactionIndexViewModel vm, TransactionDto dto)
    {
        var dataLocal = dto.Date.Kind == DateTimeKind.Utc
            ? dto.Date.ToLocalTime().Date
            : dto.Date.Date;

        vm.Input = new TransacaoFormInput
        {
            Data = dataLocal,
            TipoTransacao = (int)dto.TransactionTypeKind,
            Descricao = dto.TransactionDescription ?? "",
            Valor = dto.TransactionValue,
            CategoryId = dto.CategoryId,
            PaymentMethodId = ResolverPaymentMethodId(vm, dto.PaymentKind)
        };
        vm.AccountIdEdicao = dto.AccountId;
        vm.StatusEdicao = dto.Status;
    }

    private int ResolverPaymentMethodId(TransactionIndexViewModel vm, PaymentKind? paymentKind)
    {
        if (paymentKind is null) return 0;
        return vm.MeiosPagamentoOpcoes.FirstOrDefault(m => m.PaymentKind == paymentKind)?.Id ?? 0;
    }

    private bool ValidarFormulario(TransactionIndexViewModel vm, out TransactionTypeKind tipo, out PaymentKind? meio)
    {
        tipo = TransactionTypeKind.Receita;
        meio = null;

        if (string.IsNullOrWhiteSpace(vm.Input.Descricao) || vm.Input.Descricao.Length < 2)
        {
            vm.ErroModal = "Informe uma descrição (mín. 2 caracteres).";
            return false;
        }

        if (vm.Input.Valor <= 0)
        {
            vm.ErroModal = "Valor deve ser maior que zero.";
            return false;
        }

        if (vm.Input.CategoryId <= 0)
        {
            vm.ErroModal = "Selecione uma categoria.";
            return false;
        }

        tipo = vm.Input.TipoTransacao is (int)TransactionTypeKind.Receita or (int)TransactionTypeKind.Despesa
            ? (TransactionTypeKind)vm.Input.TipoTransacao
            : TransactionTypeKind.Receita;

        if (vm.Input.PaymentMethodId > 0)
        {
            var metodo = vm.MeiosPagamentoOpcoes.FirstOrDefault(m => m.Id == vm.Input.PaymentMethodId);
            if (metodo == null)
            {
                vm.ErroModal = "Meio de pagamento inválido.";
                return false;
            }

            meio = metodo.PaymentKind;
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
