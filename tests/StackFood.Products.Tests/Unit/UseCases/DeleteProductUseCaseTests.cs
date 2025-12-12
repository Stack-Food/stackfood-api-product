using FluentAssertions;
using Moq;
using StackFood.Products.Application.Interfaces;
using StackFood.Products.Application.UseCases;

namespace StackFood.Products.Tests.Unit.UseCases;

public class DeleteProductUseCaseTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly DeleteProductUseCase _useCase;

    public DeleteProductUseCaseTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _useCase = new DeleteProductUseCase(_productRepositoryMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldDeleteProduct_WhenProductExists()
    {
        // Arrange
        var productId = Guid.NewGuid();
        _productRepositoryMock.Setup(x => x.ExistsAsync(productId))
            .ReturnsAsync(true);

        // Act
        await _useCase.ExecuteAsync(productId);

        // Assert
        _productRepositoryMock.Verify(x => x.ExistsAsync(productId), Times.Once);
        _productRepositoryMock.Verify(x => x.DeleteAsync(productId), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowException_WhenProductDoesNotExist()
    {
        // Arrange
        var productId = Guid.NewGuid();
        _productRepositoryMock.Setup(x => x.ExistsAsync(productId))
            .ReturnsAsync(false);

        // Act
        var act = () => _useCase.ExecuteAsync(productId);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage($"Product with ID {productId} not found");
        _productRepositoryMock.Verify(x => x.ExistsAsync(productId), Times.Once);
        _productRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<Guid>()), Times.Never);
    }
}
