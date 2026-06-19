
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using FinanceControl.Contracts.Constants;
using FinanceControl.Contracts.Interfaces.Entities.Categories;
using FinanceControl.Domain.Entities.Transactions;
using FinanceControl.Domain.Entities.Users;

namespace FinanceControl.Domain.Entities.Categories;

[Table("Categories")]
public class Category : ICategory
{
    [Key]
    public Guid CategoryId { get; set; } = Guid.NewGuid();

    [Required]
    [StringLength(40)]
    public string CategoryName { get; set; } = string.Empty;

    [StringLength(255)]
    public string? Description { get; set; }

    [Required]
    [StringLength(16)]
    public string Icon { get; set; } = CategoryIcons.Default;

    public DateTimeOffset DateCreated { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? UpdatedAt { get; set; }

    public Guid? UserId { get; set; }

    public bool IsActive { get; set; } = true;

    [JsonIgnore]
    public User? User { get; set; }

    [JsonIgnore]
    public IEnumerable<Transaction>? Transactions { get; set; }
}
