using StackFood.Products.Application.DTOs;
using StackFood.Products.Application.Interfaces;
using StackFood.Products.Domain.Entities;
using StackFood.Products.Domain.ValueObjects;

namespace StackFood.Products.Application.UseCases;

public class CreateProductUseCase
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;

    public CreateProductUseCase(IProductRepository productRepository, ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<ProductDTO> ExecuteAsync(CreateProductRequest request)
    {
        // Validate category exists
        var categoryExists = await _categoryRepository.ExistsAsync(request.CategoryId);
        if (!categoryExists)
            throw new ArgumentException($"Category with ID {request.CategoryId} not found");

        var product = new Product(
            categoryId: request.CategoryId,
            name: request.Name,
            description: request.Description,
            price: new Money(request.Price),
            isAvailable: request.IsAvailable
        );

        var createdProduct = await _productRepository.CreateAsync(product);

        return new ProductDTO
        {
            Id = createdProduct.Id,
            CategoryId = createdProduct.CategoryId,
            CategoryName = createdProduct.Category?.Name ?? string.Empty,
            Name = createdProduct.Name,
            Description = createdProduct.Description,
            Price = createdProduct.Price.Amount,
            ImageUrl = createdProduct.ImageUrl,
            IsAvailable = createdProduct.IsAvailable,
            CreatedAt = createdProduct.CreatedAt
        };
    }
}
