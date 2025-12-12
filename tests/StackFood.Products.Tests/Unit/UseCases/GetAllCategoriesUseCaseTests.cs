using FluentAssertions;
using Moq;
using StackFood.Products.Application.Interfaces;
using StackFood.Products.Application.UseCases;
using StackFood.Products.Domain.Entities;

namespace StackFood.Products.Tests.Unit.UseCases;

public class GetAllCategoriesUseCaseTests
{
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly GetAllCategoriesUseCase _useCase;

    public GetAllCategoriesUseCaseTests()
    {
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _useCase = new GetAllCategoriesUseCase(_categoryRepositoryMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnAllCategories_OrderedByDisplayOrder()
    {
        // Arrange
        var categories = new List<Category>
        {
            new Category("Bebidas", "Drinks", 3),
            new Category("Lanches", "Burgers", 1),
            new Category("Sobremesas", "Desserts", 4),
            new Category("Acompanhamentos", "Sides", 2)
        };

        _categoryRepositoryMock.Setup(x => x.GetAllAsync())
            .ReturnsAsync(categories);

        // Act
        var result = (await _useCase.ExecuteAsync()).ToList();

        // Assert
        result.Should().HaveCount(4);
        result[0].Name.Should().Be("Lanches");
        result[0].DisplayOrder.Should().Be(1);
        result[1].Name.Should().Be("Acompanhamentos");
        result[1].DisplayOrder.Should().Be(2);
        result[2].Name.Should().Be("Bebidas");
        result[2].DisplayOrder.Should().Be(3);
        result[3].Name.Should().Be("Sobremesas");
        result[3].DisplayOrder.Should().Be(4);

        _categoryRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnEmptyList_WhenNoCategories()
    {
        // Arrange
        _categoryRepositoryMock.Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<Category>());

        // Act
        var result = await _useCase.ExecuteAsync();

        // Assert
        result.Should().BeEmpty();
        _categoryRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldMapAllProperties()
    {
        // Arrange
        var categories = new List<Category>
        {
            new Category("Lanches", "Delicious burgers and sandwiches", 1)
        };

        _categoryRepositoryMock.Setup(x => x.GetAllAsync())
            .ReturnsAsync(categories);

        // Act
        var result = (await _useCase.ExecuteAsync()).ToList();

        // Assert
        result.Should().HaveCount(1);
        var category = result.First();
        category.Name.Should().Be("Lanches");
        category.Description.Should().Be("Delicious burgers and sandwiches");
        category.DisplayOrder.Should().Be(1);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldHandleSingleCategory()
    {
        // Arrange
        var categories = new List<Category>
        {
            new Category("Bebidas", "Refreshing drinks", 1)
        };

        _categoryRepositoryMock.Setup(x => x.GetAllAsync())
            .ReturnsAsync(categories);

        // Act
        var result = (await _useCase.ExecuteAsync()).ToList();

        // Assert
        result.Should().HaveCount(1);
        result.First().Name.Should().Be("Bebidas");
    }
}
