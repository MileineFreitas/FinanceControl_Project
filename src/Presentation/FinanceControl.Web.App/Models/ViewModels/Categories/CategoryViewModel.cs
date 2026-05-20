using FinanceControl.Contracts.Constants;
using FinanceControl.Contracts.Dtos.Categories;

namespace FinanceControl.Web.Models.ViewModels.Categories;

public class CategoryViewModel
{
    public string? Busca { get; set; }

    public CategoryRegisterDto Input { get; set; } = new();

    public int? EditingId { get; set; }

    public bool ModalAberto { get; set; }

    public string? ErroModal { get; set; }

    public string? ErroPagina { get; set; }

    public bool UsandoDadosDemo { get; set; }

    public List<CategoryItemVm> Categorias { get; set; } = [];

    public IReadOnlyList<string> IconesDisponiveis { get; } = CategoryIcons.Available;
}

public sealed record CategoryItemVm(int? CategoryId, string Icone, string Nome, string? Descricao);

public static class CategoryViewModelMapper
{
    public static CategoryItemVm ToItem(CategoryDto dto) =>
        new(
            dto.CategoryId,
            CategoryIcons.Normalize(dto.Icon),
            dto.CategoryName ?? "—",
            dto.Description);
}
