using System.Collections.Generic;
using FinanceControl.Domain.Entities.Categories;

namespace FinanceControl.Domain.Interfaces.Repositories.Categories;

public interface ICategoryRepository
{
    IEnumerable<Category> GetCategories();

    Category GetCategoryById(int id);

    Category CreateCategory(Category category);

    Category UpdateCategory(Category category);

    Category DeleteCategory(int id);
}
