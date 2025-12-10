using FluentAssertions;
using Moq;
using StackFood.Products.Application.Interfaces;
using StackFood.Products.Application.UseCases;
using StackFood.Products.Domain.Entities;
using StackFood.Products.Domain.ValueObjects;

namespace StackFood.Products.Tests.Unit.UseCases;

public class GetAllProductsUseCaseTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly GetAllProductsUseCase _useCase;

    public GetAllProductsUseCaseTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _useCase = new GetAllProductsUseCase(_productRepositoryMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnAllProducts_WhenNoFilterProvided()
    {
        // Arrange
        var category = new Category("Lanche", "Burgers", 1);
        var products = new List<Product>
        {
            new Product(category.Id, "X-Burger", "Desc1", new Money(25m)),
            new Product(category.Id, "X-Bacon", "Desc2", new Money(28m))
        };

        _productRepositoryMock.Setup(x => x.GetAllAsync())
            .ReturnsAsync(products);

        // Act
        var result = await _useCase.ExecuteAsync();

        // Assert
        result.Should().HaveCount(2);
        _productRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnFilteredProducts_WhenCategoryNameProvided()
    {
        // Arrange
        var categoryName = "Lanche";
        var category = new Category(categoryName, "Burgers", 1);
        var products = new List<Product>
        {
            new Product(category.Id, "X-Burger", "Desc1", new Money(25m))
        };

        _productRepositoryMock.Setup(x => x.GetByCategoryNameAsync(categoryName))
            .ReturnsAsync(products);

        // Act
        var result = await _useCase.ExecuteAsync(categoryName);

        // Assert
        result.Should().HaveCount(1);
        _productRepositoryMock.Verify(x => x.GetByCategoryNameAsync(categoryName), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnOnlyAvailable_WhenAvailableOnlyIsTrue()
    {
        // Arrange
        var category = new Category("Lanche", "Burgers", 1);
        var products = new List<Product>
        {
            new Product(category.Id, "X-Burger", "Desc", new Money(25m), isAvailable: true)
        };

        _productRepositoryMock.Setup(x => x.GetAvailableAsync())
            .ReturnsAsync(products);

        // Act
        var result = await _useCase.ExecuteAsync(null, true);

        // Assert
        result.Should().HaveCount(1);
        result.All(p => p.IsAvailable).Should().BeTrue();
        _productRepositoryMock.Verify(x => x.GetAvailableAsync(), Times.Once);
    }
}
