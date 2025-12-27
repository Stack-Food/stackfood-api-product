using Moq;
using StackFood.Products.Application.Interfaces;
using StackFood.Products.Application.UseCases;
using StackFood.Products.Domain.Entities;

namespace StackFood.Products.Tests.Unit.UseCases;

public class GetCategoryByIdUseCaseTests
{
    private readonly Mock<ICategoryRepository> _mockRepository;
    private readonly GetCategoryByIdUseCase _useCase;

    public GetCategoryByIdUseCaseTests()
    {
        _mockRepository = new Mock<ICategoryRepository>();
        _useCase = new GetCategoryByIdUseCase(_mockRepository.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithExistingCategory_ShouldReturnCategoryDTO()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = new Category("Lanches", "Categoria de lanches", 1);

        _mockRepository.Setup(r => r.GetByIdAsync(categoryId))
            .ReturnsAsync(category);

        // Act
        var result = await _useCase.ExecuteAsync(categoryId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(category.Id, result.Id);
        Assert.Equal(category.Name, result.Name);
        Assert.Equal(category.Description, result.Description);
        Assert.Equal(category.DisplayOrder, result.DisplayOrder);

        _mockRepository.Verify(r => r.GetByIdAsync(categoryId), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistentCategory_ShouldReturnNull()
    {
        // Arrange
        var categoryId = Guid.NewGuid();

        _mockRepository.Setup(r => r.GetByIdAsync(categoryId))
            .ReturnsAsync((Category?)null);

        // Act
        var result = await _useCase.ExecuteAsync(categoryId);

        // Assert
        Assert.Null(result);
        _mockRepository.Verify(r => r.GetByIdAsync(categoryId), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldMapAllProperties()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = new Category("Bebidas", "Refrigerantes e sucos", 2);

        _mockRepository.Setup(r => r.GetByIdAsync(categoryId))
            .ReturnsAsync(category);

        // Act
        var result = await _useCase.ExecuteAsync(categoryId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(category.Id, result.Id);
        Assert.Equal(category.Name, result.Name);
        Assert.Equal(category.Description, result.Description);
        Assert.Equal(category.DisplayOrder, result.DisplayOrder);
    }

    [Fact]
    public async Task ExecuteAsync_WithDifferentCategories_ShouldReturnCorrectData()
    {
        // Arrange
        var categoryId1 = Guid.NewGuid();
        var categoryId2 = Guid.NewGuid();

        var category1 = new Category("Lanches", "Hambúrgueres", 1);
        var category2 = new Category("Sobremesas", "Doces", 3);

        _mockRepository.Setup(r => r.GetByIdAsync(categoryId1))
            .ReturnsAsync(category1);

        _mockRepository.Setup(r => r.GetByIdAsync(categoryId2))
            .ReturnsAsync(category2);

        // Act
        var result1 = await _useCase.ExecuteAsync(categoryId1);
        var result2 = await _useCase.ExecuteAsync(categoryId2);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal("Lanches", result1.Name);
        Assert.Equal("Sobremesas", result2.Name);
    }
}
