using StackFood.Products.Domain.Entities;

namespace StackFood.Products.Application.Interfaces;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id);
    Task<IEnumerable<Product>> GetAllAsync();
    Task<IEnumerable<Product>> GetByCategoryIdAsync(Guid categoryId);
    Task<IEnumerable<Product>> GetByCategoryNameAsync(string categoryName);
    Task<IEnumerable<Product>> GetAvailableAsync();
    Task<Product> CreateAsync(Product product);
    Task<Product> UpdateAsync(Product product);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}
