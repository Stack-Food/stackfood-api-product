using FluentAssertions;
using Moq;
using StackFood.Products.Application.DTOs;
using StackFood.Products.Application.Interfaces;
using StackFood.Products.Application.UseCases;
using StackFood.Products.Domain.Entities;
using StackFood.Products.Domain.ValueObjects;

namespace StackFood.Products.Tests.Unit.UseCases;

public class CreateProductUseCaseTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly CreateProductUseCase _useCase;

    public CreateProductUseCaseTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _useCase = new CreateProductUseCase(_productRepositoryMock.Object, _categoryRepositoryMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCreateProduct_WhenValidRequest()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var request = new CreateProductRequest
        {
            CategoryId = categoryId,
            Name = "X-Burger",
            Description = "Delicious burger",
            Price = 25.90m,
            IsAvailable = true
        };

        _categoryRepositoryMock.Setup(x => x.ExistsAsync(categoryId))
            .ReturnsAsync(true);

        _productRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<Product>()))
            .ReturnsAsync((Product p) => p);

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("X-Burger");
        result.Price.Should().Be(25.90m);
        _categoryRepositoryMock.Verify(x => x.ExistsAsync(categoryId), Times.Once);
        _productRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowException_WhenCategoryDoesNotExist()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var request = new CreateProductRequest
        {
            CategoryId = categoryId,
            Name = "X-Burger",
            Price = 25.90m
        };

        _categoryRepositoryMock.Setup(x => x.ExistsAsync(categoryId))
            .ReturnsAsync(false);

        // Act
        Func<Task> act = async () => await _useCase.ExecuteAsync(request);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage($"Category with ID {categoryId} not found");
    }
}
