using FluentAssertions;
using Moq;
using StackFood.Products.Application.DTOs;
using StackFood.Products.Application.Interfaces;
using StackFood.Products.Application.UseCases;
using StackFood.Products.Domain.Entities;
using StackFood.Products.Domain.ValueObjects;

namespace StackFood.Products.Tests.Unit.UseCases;

public class UpdateProductUseCaseTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly UpdateProductUseCase _useCase;

    public UpdateProductUseCaseTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _useCase = new UpdateProductUseCase(_productRepositoryMock.Object, _categoryRepositoryMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldUpdateProduct_WhenValidRequest()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var product = new Product(
            categoryId,
            "Old Name",
            "Old Description",
            new Money(10.00m),
            "https://old.com/image.jpg"
        );

        var request = new UpdateProductRequest
        {
            CategoryId = categoryId,
            Name = "New Name",
            Description = "New Description",
            Price = 20.00m,
            ImageUrl = "https://new.com/image.jpg"
        };

        _productRepositoryMock.Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync(product);

        _categoryRepositoryMock.Setup(x => x.ExistsAsync(categoryId))
            .ReturnsAsync(true);

        _productRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Product>()))
            .ReturnsAsync((Product p) => p);

        // Act
        var result = await _useCase.ExecuteAsync(productId, request);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("New Name");
        result.Description.Should().Be("New Description");
        result.Price.Should().Be(20.00m);
        result.ImageUrl.Should().Be("https://new.com/image.jpg");

        _productRepositoryMock.Verify(x => x.GetByIdAsync(productId), Times.Once);
        _categoryRepositoryMock.Verify(x => x.ExistsAsync(categoryId), Times.Once);
        _productRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowException_WhenProductDoesNotExist()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var request = new UpdateProductRequest
        {
            CategoryId = Guid.NewGuid(),
            Name = "New Name",
            Description = "New Description",
            Price = 20.00m
        };

        _productRepositoryMock.Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync((Product?)null);

        // Act
        var act = () => _useCase.ExecuteAsync(productId, request);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage($"Product with ID {productId} not found");

        _productRepositoryMock.Verify(x => x.GetByIdAsync(productId), Times.Once);
        _categoryRepositoryMock.Verify(x => x.ExistsAsync(It.IsAny<Guid>()), Times.Never);
        _productRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowException_WhenCategoryDoesNotExist()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var product = new Product(
            Guid.NewGuid(),
            "Old Name",
            "Old Description",
            new Money(10.00m),
            null
        );

        var request = new UpdateProductRequest
        {
            CategoryId = categoryId,
            Name = "New Name",
            Description = "New Description",
            Price = 20.00m
        };

        _productRepositoryMock.Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync(product);

        _categoryRepositoryMock.Setup(x => x.ExistsAsync(categoryId))
            .ReturnsAsync(false);

        // Act
        var act = () => _useCase.ExecuteAsync(productId, request);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage($"Category with ID {categoryId} not found");

        _productRepositoryMock.Verify(x => x.GetByIdAsync(productId), Times.Once);
        _categoryRepositoryMock.Verify(x => x.ExistsAsync(categoryId), Times.Once);
        _productRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Product>()), Times.Never);
    }
}
