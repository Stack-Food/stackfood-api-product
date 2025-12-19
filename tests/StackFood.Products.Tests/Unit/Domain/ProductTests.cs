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

    [Fact]
    public void SetImageUrl_ShouldUpdateImageUrl()
    {
        // Arrange
        var product = new Product(Guid.NewGuid(), "Product", "Description", new Money(10m));

        // Act
        product.SetImageUrl("https://example.com/image.jpg");

        // Assert
        product.ImageUrl.Should().Be("https://example.com/image.jpg");
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenNameIsEmpty()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var price = new Money(25.90m);

        // Act
        Action act = () => new Product(categoryId, "", "Description", price);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Product name cannot be empty*");
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenNameIsWhitespace()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var price = new Money(25.90m);

        // Act
        Action act = () => new Product(categoryId, "   ", "Description", price);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Product name cannot be empty*");
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenNameIsTooLong()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var price = new Money(25.90m);
        var longName = new string('A', 201);

        // Act
        Action act = () => new Product(categoryId, longName, "Description", price);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Product name cannot exceed 200 characters*");
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenCategoryIdIsEmpty()
    {
        // Arrange
        var price = new Money(25.90m);

        // Act
        Action act = () => new Product(Guid.Empty, "Product", "Description", price);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Category ID cannot be empty*");
    }

    [Fact]
    public void Update_ShouldThrowException_WhenNameIsNull()
    {
        // Arrange
        var product = new Product(Guid.NewGuid(), "Product", "Description", new Money(10m));

        // Act
        Action act = () => product.Update(Guid.NewGuid(), null!, "Description", new Money(10m), null);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Update_ShouldThrowException_WhenPriceIsNull()
    {
        // Arrange
        var product = new Product(Guid.NewGuid(), "Product", "Description", new Money(10m));

        // Act
        Action act = () => product.Update(Guid.NewGuid(), "New Name", "Description", null!, null);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Update_ShouldThrowException_WhenNameIsEmpty()
    {
        // Arrange
        var product = new Product(Guid.NewGuid(), "Product", "Description", new Money(10m));

        // Act
        Action act = () => product.Update(Guid.NewGuid(), "", "Description", new Money(10m), null);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Product name cannot be empty*");
    }

    [Fact]
    public void Update_ShouldThrowException_WhenCategoryIdIsEmpty()
    {
        // Arrange
        var product = new Product(Guid.NewGuid(), "Product", "Description", new Money(10m));

        // Act
        Action act = () => product.Update(Guid.Empty, "New Name", "Description", new Money(10m), null);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Category ID cannot be empty*");
    }

    [Fact]
    public void Constructor_WithOptionalParameters_ShouldSetDefaults()
    {
        // Arrange & Act
        var product = new Product(
            Guid.NewGuid(),
            "Product",
            "Description",
            new Money(10m),
            imageUrl: "image.jpg",
            isAvailable: false
        );

        // Assert
        product.ImageUrl.Should().Be("image.jpg");
        product.IsAvailable.Should().BeFalse();
    }

    [Fact]
    public void Constructor_WithNullDescription_ShouldSucceed()
    {
        // Arrange & Act
        var product = new Product(
            Guid.NewGuid(),
            "Product",
            null,
            new Money(10m)
        );

        // Assert
        product.Description.Should().BeNull();
    }
}
