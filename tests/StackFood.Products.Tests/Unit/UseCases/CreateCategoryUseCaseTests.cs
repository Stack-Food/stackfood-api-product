using Moq;
using StackFood.Products.Application.DTOs;
using StackFood.Products.Application.Interfaces;
using StackFood.Products.Application.UseCases;
using StackFood.Products.Domain.Entities;

namespace StackFood.Products.Tests.Unit.UseCases;

public class CreateCategoryUseCaseTests
{
    private readonly Mock<ICategoryRepository> _mockRepository;
    private readonly CreateCategoryUseCase _useCase;

    public CreateCategoryUseCaseTests()
    {
        _mockRepository = new Mock<ICategoryRepository>();
        _useCase = new CreateCategoryUseCase(_mockRepository.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidRequest_ShouldCreateCategory()
    {
        // Arrange
        var request = new CreateCategoryRequest
        {
            Name = "Lanches",
            Description = "Categoria de lanches",
            DisplayOrder = 1
        };

        var category = new Category(request.Name, request.Description, request.DisplayOrder);

        _mockRepository.Setup(r => r.GetByNameAsync(request.Name))
            .ReturnsAsync((Category?)null);

        _mockRepository.Setup(r => r.CreateAsync(It.IsAny<Category>()))
            .ReturnsAsync(category);

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.Name, result.Name);
        Assert.Equal(request.Description, result.Description);
        Assert.Equal(request.DisplayOrder, result.DisplayOrder);

        _mockRepository.Verify(r => r.GetByNameAsync(request.Name), Times.Once);
        _mockRepository.Verify(r => r.CreateAsync(It.IsAny<Category>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithDuplicateName_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var request = new CreateCategoryRequest
        {
            Name = "Lanches",
            Description = "Categoria de lanches",
            DisplayOrder = 1
        };

        var existingCategory = new Category("Lanches", "Existing", 1);

        _mockRepository.Setup(r => r.GetByNameAsync(request.Name))
            .ReturnsAsync(existingCategory);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _useCase.ExecuteAsync(request));

        Assert.Contains("already exists", exception.Message);
        _mockRepository.Verify(r => r.GetByNameAsync(request.Name), Times.Once);
        _mockRepository.Verify(r => r.CreateAsync(It.IsAny<Category>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldMapAllProperties()
    {
        // Arrange
        var request = new CreateCategoryRequest
        {
            Name = "Bebidas",
            Description = "Refrigerantes e sucos",
            DisplayOrder = 2
        };

        var category = new Category(request.Name, request.Description, request.DisplayOrder);

        _mockRepository.Setup(r => r.GetByNameAsync(It.IsAny<string>()))
            .ReturnsAsync((Category?)null);

        _mockRepository.Setup(r => r.CreateAsync(It.IsAny<Category>()))
            .ReturnsAsync(category);

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        Assert.Equal(category.Id, result.Id);
        Assert.Equal(category.Name, result.Name);
        Assert.Equal(category.Description, result.Description);
        Assert.Equal(category.DisplayOrder, result.DisplayOrder);
    }
}
