using StackFood.Products.Application.DTOs;
using StackFood.Products.Application.Interfaces;

namespace StackFood.Products.Application.UseCases;

public class GetAllProductsUseCase
{
    private readonly IProductRepository _productRepository;

    public GetAllProductsUseCase(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<IEnumerable<ProductDTO>> ExecuteAsync(string? categoryName = null, bool? availableOnly = null)
    {
        IEnumerable<Domain.Entities.Product> products;

        if (!string.IsNullOrWhiteSpace(categoryName))
        {
            products = await _productRepository.GetByCategoryNameAsync(categoryName);
        }
        else if (availableOnly == true)
        {
            products = await _productRepository.GetAvailableAsync();
        }
        else
        {
            products = await _productRepository.GetAllAsync();
        }

        return products.Select(p => new ProductDTO
        {
            Id = p.Id,
            CategoryId = p.CategoryId,
            CategoryName = p.Category?.Name ?? string.Empty,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price.Amount,
            ImageUrl = p.ImageUrl,
            IsAvailable = p.IsAvailable,
            CreatedAt = p.CreatedAt
        });
    }
}
