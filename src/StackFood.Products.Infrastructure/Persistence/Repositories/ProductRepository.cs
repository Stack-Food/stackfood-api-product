using Microsoft.EntityFrameworkCore;
using StackFood.Products.Application.Interfaces;
using StackFood.Products.Domain.Entities;
using System.Diagnostics.CodeAnalysis;

namespace StackFood.Products.Infrastructure.Persistence.Repositories;

[ExcludeFromCodeCoverage]
public class ProductRepository : IProductRepository
{
    private readonly ProductsDbContext _context;

    public ProductRepository(ProductsDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(Guid id)
    {
        return await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _context.Products
            .Include(p => p.Category)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetByCategoryIdAsync(Guid categoryId)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Where(p => p.CategoryId == categoryId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetByCategoryNameAsync(string categoryName)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Where(p => p.Category.Name == categoryName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetAvailableAsync()
    {
        return await _context.Products
            .Include(p => p.Category)
            .Where(p => p.IsAvailable)
            .ToListAsync();
    }

    public async Task<Product> CreateAsync(Product product)
    {
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<Product> UpdateAsync(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task DeleteAsync(Guid id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Products.AnyAsync(p => p.Id == id);
    }
}
