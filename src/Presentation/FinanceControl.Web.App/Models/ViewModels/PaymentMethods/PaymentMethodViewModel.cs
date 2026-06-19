using FinanceControl.Contracts.Constants;
using FinanceControl.Contracts.Dtos.PaymentMethods;

namespace FinanceControl.Web.Models.ViewModels.PaymentMethods;

public class PaymentMethodViewModel
{
    public string? Busca { get; set; }

    public PaymentMethodCreateDto Input { get; set; } = new();

    public Guid? EditingId { get; set; }

    public bool ModalAberto { get; set; }

    public string? ErroModal { get; set; }

    public string? ErroPagina { get; set; }

    public List<PaymentMethodItemVm> Meios { get; set; } = [];

    public IReadOnlyList<string> IconesDisponiveis { get; } = PaymentMethodIcons.Available;
}

public sealed record PaymentMethodItemVm(
    Guid Id,
    string Icone,
    string Nome,
    string Status,
    string? Descricao = null);

public static class PaymentMethodViewModelMapper
{
    public static PaymentMethodItemVm ToItem(PaymentMethodDto dto) =>
        new(
            dto.PaymentMethodId,
            PaymentMethodIcons.Normalize(dto.Icon),
            dto.Name,
            dto.IsActive ? "Ativo" : "Inativo",
            dto.Description);
}
