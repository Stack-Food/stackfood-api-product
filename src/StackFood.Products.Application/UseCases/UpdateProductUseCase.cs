using StackFood.Products.Application.DTOs;
using StackFood.Products.Application.Interfaces;
using StackFood.Products.Domain.ValueObjects;

namespace StackFood.Products.Application.UseCases;

public class UpdateProductUseCase
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;

    public UpdateProductUseCase(IProductRepository productRepository, ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<ProductDTO> ExecuteAsync(Guid id, UpdateProductRequest request)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            throw new ArgumentException($"Product with ID {id} not found");

        // Validate category exists
        var categoryExists = await _categoryRepository.ExistsAsync(request.CategoryId);
        if (!categoryExists)
            throw new ArgumentException($"Category with ID {request.CategoryId} not found");

        product.Update(
            categoryId: request.CategoryId,
            name: request.Name,
            description: request.Description,
            price: new Money(request.Price),
            imageUrl: request.ImageUrl
        );

        var updatedProduct = await _productRepository.UpdateAsync(product);

        return new ProductDTO
        {
            Id = updatedProduct.Id,
            CategoryId = updatedProduct.CategoryId,
            CategoryName = updatedProduct.Category?.Name ?? string.Empty,
            Name = updatedProduct.Name,
            Description = updatedProduct.Description,
            Price = updatedProduct.Price.Amount,
            ImageUrl = updatedProduct.ImageUrl,
            IsAvailable = updatedProduct.IsAvailable,
            CreatedAt = updatedProduct.CreatedAt
        };
    }
}
