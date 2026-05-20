using FinanceControl.Contracts.Constants;
using FinanceControl.Contracts.Dtos.TransactionTypes;

namespace FinanceControl.Web.Models.ViewModels.TransactionTypes;

public class TransactionTypeViewModel
{
    public string? Busca { get; set; }

    public TransactionTypeCreateDto Input { get; set; } = new();

    public int? EditingId { get; set; }

    public bool ModalAberto { get; set; }

    public string? ErroModal { get; set; }

    public string? ErroPagina { get; set; }

    public bool UsandoDadosDemo { get; set; }

    public List<TransactionTypeItemVm> Tipos { get; set; } = [];

    public IReadOnlyList<string> IconesDisponiveis { get; } = PaymentMethodIcons.Available;
}

public sealed record TransactionTypeItemVm(
    int Id,
    string Icone,
    string Nome,
    string Codigo,
    string MeioPagamento,
    string Status,
    bool IsSystem,
    string? Descricao = null);

public static class TransactionTypeViewModelMapper
{
    public static TransactionTypeItemVm ToItem(TransactionTypeDto dto) =>
        new(
            dto.TransactionTypeId,
            PaymentMethodIcons.Normalize(dto.Icon),
            dto.Name,
            dto.Code,
            dto.PaymentKindLabel,
            dto.IsActive ? "Ativo" : "Inativo",
            dto.IsSystem,
            dto.Description);
}
