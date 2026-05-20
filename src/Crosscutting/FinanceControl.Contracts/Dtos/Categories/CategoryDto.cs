using FinanceControl.Contracts.Interfaces.Entities.Categories;

namespace FinanceControl.Contracts.Dtos.Categories;

public class CategoryDto : ICategory
{
    public int CategoryId { get; set; }

    public string? CategoryName { get; set; }

    public string? Description { get; set; }

    public string Icon { get; set; } = string.Empty;

    public DateTime DateCreated { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? UserId { get; set; }
}
