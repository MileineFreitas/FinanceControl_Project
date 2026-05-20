namespace FinanceControl.Contracts.Interfaces.Entities.Categories;

public interface ICategory
{
    int CategoryId { get; set; }

    string? CategoryName { get; set; }

    string? Description { get; set; }

    string Icon { get; set; }

    DateTime DateCreated { get; set; }

    DateTime? UpdatedAt { get; set; }

    int? UserId { get; set; }
}
