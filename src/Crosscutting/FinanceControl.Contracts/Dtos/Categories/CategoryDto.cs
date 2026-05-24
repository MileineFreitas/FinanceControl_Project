namespace FinanceControl.Contracts.Dtos.Categories;

public class CategoryDto
{
    public Guid CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string Icon { get; set; } = string.Empty;

    public DateTimeOffset DateCreated { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }

    public int? UserId { get; set; }
}
