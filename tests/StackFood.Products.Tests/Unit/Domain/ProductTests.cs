using FluentAssertions;
using StackFood.Products.Domain.Entities;
using StackFood.Products.Domain.ValueObjects;

namespace StackFood.Products.Tests.Unit.Domain;

public class ProductTests
{
    [Fact]
    public void Constructor_ShouldCreateValidProduct()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var price = new Money(25.90m);

        // Act
        var product = new Product(categoryId, "X-Burger", "Delicious burger", price);

        // Assert
        product.Id.Should().NotBeEmpty();
        product.CategoryId.Should().Be(categoryId);
        product.Name.Should().Be("X-Burger");
        product.Description.Should().Be("Delicious burger");
        product.Price.Should().Be(price);
        product.IsAvailable.Should().BeTrue();
        product.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenNameIsNull()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var price = new Money(25.90m);

        // Act
        Action act = () => new Product(categoryId, null!, "Description", price);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenPriceIsNull()
    {
        // Arrange
        var categoryId = Guid.NewGuid();

        // Act
        Action act = () => new Product(categoryId, "Product", "Description", null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Update_ShouldUpdateProductProperties()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var newCategoryId = Guid.NewGuid();
        var product = new Product(categoryId, "Old Name", "Old Description", new Money(10m));
        var oldUpdatedAt = product.UpdatedAt;

        // Act
        Thread.Sleep(10);
        product.Update(newCategoryId, "New Name", "New Description", new Money(20m), "image.jpg");

        // Assert
        product.CategoryId.Should().Be(newCategoryId);
        product.Name.Should().Be("New Name");
        product.Description.Should().Be("New Description");
        product.Price.Amount.Should().Be(20m);
        product.ImageUrl.Should().Be("image.jpg");
        product.UpdatedAt.Should().BeAfter(oldUpdatedAt);
    }

    [Fact]
    public void SetAvailability_ShouldUpdateAvailability()
    {
        // Arrange
        var product = new Product(Guid.NewGuid(), "Product", "Description", new Money(10m), isAvailable: true);

        // Act
        product.SetAvailability(false);

        // Assert
        product.IsAvailable.Should().BeFalse();
    }
}
