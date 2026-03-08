using System;
using System.Collections.Generic;
using System.Linq;
using FinanceControl.Domain.Entities.Categories;
using FinanceControl.Domain.Interfaces.Repositories.Categories;
using FinanceControl.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace FinanceControl.Infrastructure.Repositories.Categories;

public class CategoryRepository : ICategoryRepository
{
    private readonly FinanceDbContext _context;

    public CategoryRepository(FinanceDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Category> GetCategories()
    {
        var categories = _context.Categories.AsNoTracking().ToList();
        if (categories == null)
            throw new KeyNotFoundException("Categorias não encontradas");
        return categories;
    }

    public Category GetCategoryById(int id)
    {
        var category = _context.Categories.AsNoTracking().FirstOrDefault(c => c.CategoryId == id);
        if (category == null)
            throw new KeyNotFoundException($"Categoria com id:{id} não localizada...");
        return category;
    }

    public Category CreateCategory(Category category)
    {
        if (category == null)
            throw new ArgumentException(nameof(category), "Erro ao criar categoria");

        _context.Categories.Add(category);
        _context.SaveChanges();
        return category;
    }

    public Category UpdateCategory(Category category)
    {
        if (category == null)
            throw new ArgumentException(nameof(category));

        _context.Entry(category).State = EntityState.Modified;
        _context.SaveChanges();
        return category;
    }

    public Category DeleteCategory(int id)
    {
        var category = _context.Categories.Find(id);
        if (category == null)
            throw new KeyNotFoundException(nameof(category));

        _context.Categories.Remove(category);
        _context.SaveChanges();
        return category;
    }
}
