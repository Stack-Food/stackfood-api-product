using StackFood.Products.Application.DTOs;
using StackFood.Products.Application.Interfaces;

namespace StackFood.Products.Application.UseCases;

public class GetProductByIdUseCase
{
    private readonly IProductRepository _productRepository;

    public GetProductByIdUseCase(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductDTO?> ExecuteAsync(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product == null)
            return null;

        return new ProductDTO
        {
            Id = product.Id,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name ?? string.Empty,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price.Amount,
            ImageUrl = product.ImageUrl,
            IsAvailable = product.IsAvailable,
            CreatedAt = product.CreatedAt
        };
    }
}
