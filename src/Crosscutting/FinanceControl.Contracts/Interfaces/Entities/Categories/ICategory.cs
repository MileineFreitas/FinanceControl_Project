namespace FinanceControl.Contracts.Interfaces.Entities.Categories;

public interface ICategory
{
    Guid CategoryId { get; set; }

    string CategoryName { get; set; }

    string? Description { get; set; }

    string Icon { get; set; }

    DateTimeOffset DateCreated { get; set; }

    DateTimeOffset? UpdatedAt { get; set; }

    int? UserId { get; set; }
}
