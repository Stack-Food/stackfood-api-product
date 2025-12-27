using Moq;
using StackFood.Products.Application.Interfaces;
using StackFood.Products.Application.UseCases;

namespace StackFood.Products.Tests.Unit.UseCases;

public class DeleteCategoryUseCaseTests
{
    private readonly Mock<ICategoryRepository> _mockRepository;
    private readonly DeleteCategoryUseCase _useCase;

    public DeleteCategoryUseCaseTests()
    {
        _mockRepository = new Mock<ICategoryRepository>();
        _useCase = new DeleteCategoryUseCase(_mockRepository.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithExistingCategory_ShouldDeleteSuccessfully()
    {
        // Arrange
        var categoryId = Guid.NewGuid();

        _mockRepository.Setup(r => r.ExistsAsync(categoryId))
            .ReturnsAsync(true);

        _mockRepository.Setup(r => r.DeleteAsync(categoryId))
            .Returns(Task.CompletedTask);

        // Act
        await _useCase.ExecuteAsync(categoryId);

        // Assert
        _mockRepository.Verify(r => r.ExistsAsync(categoryId), Times.Once);
        _mockRepository.Verify(r => r.DeleteAsync(categoryId), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistentCategory_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var categoryId = Guid.NewGuid();

        _mockRepository.Setup(r => r.ExistsAsync(categoryId))
            .ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _useCase.ExecuteAsync(categoryId));

        Assert.Contains("not found", exception.Message);
        Assert.Contains(categoryId.ToString(), exception.Message);

        _mockRepository.Verify(r => r.ExistsAsync(categoryId), Times.Once);
        _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCheckExistenceBeforeDeleting()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var callOrder = new List<string>();

        _mockRepository.Setup(r => r.ExistsAsync(categoryId))
            .ReturnsAsync(true)
            .Callback(() => callOrder.Add("Exists"));

        _mockRepository.Setup(r => r.DeleteAsync(categoryId))
            .Returns(Task.CompletedTask)
            .Callback(() => callOrder.Add("Delete"));

        // Act
        await _useCase.ExecuteAsync(categoryId);

        // Assert
        Assert.Equal(2, callOrder.Count);
        Assert.Equal("Exists", callOrder[0]);
        Assert.Equal("Delete", callOrder[1]);
    }
}
