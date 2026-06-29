using FinanceControl.Contracts.Constants;
using FinanceControl.Contracts.Dtos.Categories;

namespace FinanceControl.Web.Models.ViewModels.Categories;

public class CategoryViewModel
{
    public string? Busca { get; set; }

    public CategoryRegisterDto Input { get; set; } = new();

    public Guid? EditingId { get; set; }

    public bool ModalAberto { get; set; }

    public string? ErroModal { get; set; }

    public string? ErroPagina { get; set; }

    public List<CategoryItemVm> Categorias { get; set; } = [];

    public IReadOnlyList<string> IconesDisponiveis { get; } = CategoryIcons.Available;
}

public sealed record CategoryItemVm(Guid? CategoryId, string Icone, string Nome, string Status, string? Descricao);

public static class CategoryViewModelMapper
{
    public static CategoryItemVm ToItem(CategoryDto dto) =>
        new(
            dto.CategoryId,
            CategoryIcons.Normalize(dto.Icon),
            dto.CategoryName ?? "—",
            dto.IsActive ? "Ativo" : "Inativo",
            dto.Description);
}
