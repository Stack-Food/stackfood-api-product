using Moq;
using StackFood.Products.Application.DTOs;
using StackFood.Products.Application.Interfaces;
using StackFood.Products.Application.UseCases;
using StackFood.Products.Domain.Entities;

namespace StackFood.Products.Tests.Unit.UseCases;

public class UpdateCategoryUseCaseTests
{
    private readonly Mock<ICategoryRepository> _mockRepository;
    private readonly UpdateCategoryUseCase _useCase;

    public UpdateCategoryUseCaseTests()
    {
        _mockRepository = new Mock<ICategoryRepository>();
        _useCase = new UpdateCategoryUseCase(_mockRepository.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidRequest_ShouldUpdateCategory()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var request = new UpdateCategoryRequest
        {
            Name = "Lanches Atualizados",
            Description = "Nova descrição",
            DisplayOrder = 2
        };

        var existingCategory = new Category("Lanches", "Descrição antiga", 1);
        var updatedCategory = new Category(request.Name, request.Description, request.DisplayOrder);

        _mockRepository.Setup(r => r.GetByIdAsync(categoryId))
            .ReturnsAsync(existingCategory);

        _mockRepository.Setup(r => r.GetByNameAsync(request.Name))
            .ReturnsAsync((Category?)null);

        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Category>()))
            .ReturnsAsync(updatedCategory);

        // Act
        var result = await _useCase.ExecuteAsync(categoryId, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.Name, result.Name);
        Assert.Equal(request.Description, result.Description);
        Assert.Equal(request.DisplayOrder, result.DisplayOrder);

        _mockRepository.Verify(r => r.GetByIdAsync(categoryId), Times.Once);
        _mockRepository.Verify(r => r.GetByNameAsync(request.Name), Times.Once);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Category>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistentCategory_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var request = new UpdateCategoryRequest
        {
            Name = "Lanches",
            Description = "Descrição",
            DisplayOrder = 1
        };

        _mockRepository.Setup(r => r.GetByIdAsync(categoryId))
            .ReturnsAsync((Category?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _useCase.ExecuteAsync(categoryId, request));

        Assert.Contains("not found", exception.Message);
        Assert.Contains(categoryId.ToString(), exception.Message);

        _mockRepository.Verify(r => r.GetByIdAsync(categoryId), Times.Once);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Category>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithDuplicateName_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var request = new UpdateCategoryRequest
        {
            Name = "Bebidas",
            Description = "Descrição",
            DisplayOrder = 1
        };

        var existingCategory = new Category("Lanches", "Descrição antiga", 1);
        var anotherCategory = new Category("Bebidas", "Outra categoria", 2);

        _mockRepository.Setup(r => r.GetByIdAsync(categoryId))
            .ReturnsAsync(existingCategory);

        _mockRepository.Setup(r => r.GetByNameAsync(request.Name))
            .ReturnsAsync(anotherCategory);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _useCase.ExecuteAsync(categoryId, request));

        Assert.Contains("already exists", exception.Message);

        _mockRepository.Verify(r => r.GetByIdAsync(categoryId), Times.Once);
        _mockRepository.Verify(r => r.GetByNameAsync(request.Name), Times.Once);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Category>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenNameNotChanged_ShouldAllowUpdate()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var request = new UpdateCategoryRequest
        {
            Name = "Lanches",
            Description = "Nova descrição",
            DisplayOrder = 2
        };

        var existingCategory = new Category("Lanches", "Descrição antiga", 1);
        var updatedCategory = new Category(request.Name, request.Description, request.DisplayOrder);

        _mockRepository.Setup(r => r.GetByIdAsync(categoryId))
            .ReturnsAsync(existingCategory);

        // Nome não existe em outra categoria
        _mockRepository.Setup(r => r.GetByNameAsync(request.Name))
            .ReturnsAsync((Category?)null);

        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Category>()))
            .ReturnsAsync(updatedCategory);

        // Act
        var result = await _useCase.ExecuteAsync(categoryId, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.Name, result.Name);
        Assert.Equal(request.Description, result.Description);

        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Category>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldMapAllProperties()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var request = new UpdateCategoryRequest
        {
            Name = "Sobremesas",
            Description = "Doces e sobremesas",
            DisplayOrder = 3
        };

        var existingCategory = new Category("Lanches", "Old", 1);
        var updatedCategory = new Category(request.Name, request.Description, request.DisplayOrder);

        _mockRepository.Setup(r => r.GetByIdAsync(categoryId))
            .ReturnsAsync(existingCategory);

        _mockRepository.Setup(r => r.GetByNameAsync(It.IsAny<string>()))
            .ReturnsAsync((Category?)null);

        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Category>()))
            .ReturnsAsync(updatedCategory);

        // Act
        var result = await _useCase.ExecuteAsync(categoryId, request);

        // Assert
        Assert.Equal(updatedCategory.Id, result.Id);
        Assert.Equal(updatedCategory.Name, result.Name);
        Assert.Equal(updatedCategory.Description, result.Description);
        Assert.Equal(updatedCategory.DisplayOrder, result.DisplayOrder);
    }
}
