using FluentAssertions;
using StackFood.Products.Domain.Entities;

namespace StackFood.Products.Tests.Unit.Domain;

public class CategoryTests
{
    [Fact]
    public void Constructor_ShouldCreateValidCategory()
    {
        // Arrange & Act
        var category = new Category("Lanche", "Hambúrgueres", 1);

        // Assert
        category.Id.Should().NotBeEmpty();
        category.Name.Should().Be("Lanche");
        category.Description.Should().Be("Hambúrgueres");
        category.DisplayOrder.Should().Be(1);
        category.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        category.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenNameIsNull()
    {
        // Arrange & Act
        Action act = () => new Category(null!, "Description", 1);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenNameIsEmpty()
    {
        // Arrange & Act
        Action act = () => new Category("", "Description", 1);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenDisplayOrderIsNegative()
    {
        // Arrange & Act
        Action act = () => new Category("Lanche", "Description", -1);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Update_ShouldUpdateCategoryProperties()
    {
        // Arrange
        var category = new Category("Lanche", "Old Description", 1);
        var oldUpdatedAt = category.UpdatedAt;

        // Act
        Thread.Sleep(10); // Ensure time difference
        category.Update("Bebida", "New Description", 2);

        // Assert
        category.Name.Should().Be("Bebida");
        category.Description.Should().Be("New Description");
        category.DisplayOrder.Should().Be(2);
        category.UpdatedAt.Should().BeAfter(oldUpdatedAt);
    }
}
