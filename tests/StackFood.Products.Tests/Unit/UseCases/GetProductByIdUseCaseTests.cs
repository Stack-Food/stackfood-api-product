using FluentAssertions;
using Moq;
using StackFood.Products.Application.Interfaces;
using StackFood.Products.Application.UseCases;
using StackFood.Products.Domain.Entities;
using StackFood.Products.Domain.ValueObjects;

namespace StackFood.Products.Tests.Unit.UseCases;

public class GetProductByIdUseCaseTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly GetProductByIdUseCase _useCase;

    public GetProductByIdUseCaseTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _useCase = new GetProductByIdUseCase(_productRepositoryMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnProductDTO_WhenProductExists()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var category = new Category("Burgers", "Delicious burgers", 1);
        var product = new Product(
            categoryId,
            "X-Burger",
            "Delicious burger",
            new Money(25.90m),
            "https://example.com/burger.jpg"
        );

        // Set category via reflection
        var categoryProperty = typeof(Product).GetProperty("Category");
        categoryProperty?.SetValue(product, category);

        _productRepositoryMock.Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync(product);

        // Act
        var result = await _useCase.ExecuteAsync(productId);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("X-Burger");
        result.Price.Should().Be(25.90m);
        result.CategoryId.Should().Be(categoryId);
        result.CategoryName.Should().Be("Burgers");
        result.IsAvailable.Should().BeTrue();
        _productRepositoryMock.Verify(x => x.GetByIdAsync(productId), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnNull_WhenProductDoesNotExist()
    {
        // Arrange
        var productId = Guid.NewGuid();
        _productRepositoryMock.Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _useCase.ExecuteAsync(productId);

        // Assert
        result.Should().BeNull();
        _productRepositoryMock.Verify(x => x.GetByIdAsync(productId), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldMapAllProperties_WhenProductExists()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var product = new Product(
            categoryId,
            "Fries",
            "Crispy fries",
            new Money(12.50m),
            "https://example.com/fries.jpg"
        );

        _productRepositoryMock.Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync(product);

        // Act
        var result = await _useCase.ExecuteAsync(productId);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Fries");
        result.Description.Should().Be("Crispy fries");
        result.Price.Should().Be(12.50m);
        result.ImageUrl.Should().Be("https://example.com/fries.jpg");
    }
}
