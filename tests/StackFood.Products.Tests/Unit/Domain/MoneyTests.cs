using FluentAssertions;
using StackFood.Products.Domain.ValueObjects;

namespace StackFood.Products.Tests.Unit.Domain;

public class MoneyTests
{
    [Fact]
    public void Constructor_ShouldCreateValidMoney()
    {
        // Arrange & Act
        var money = new Money(25.90m);

        // Assert
        money.Amount.Should().Be(25.90m);
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenAmountIsNegative()
    {
        // Arrange & Act
        Action act = () => new Money(-10m);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_ShouldRoundToTwoDecimalPlaces()
    {
        // Arrange & Act
        var money = new Money(25.999m);

        // Assert
        money.Amount.Should().Be(26.00m);
    }

    [Fact]
    public void Addition_ShouldReturnCorrectSum()
    {
        // Arrange
        var money1 = new Money(10m);
        var money2 = new Money(20m);

        // Act
        var result = money1 + money2;

        // Assert
        result.Amount.Should().Be(30m);
    }

    [Fact]
    public void Subtraction_ShouldReturnCorrectDifference()
    {
        // Arrange
        var money1 = new Money(30m);
        var money2 = new Money(10m);

        // Act
        var result = money1 - money2;

        // Assert
        result.Amount.Should().Be(20m);
    }

    [Fact]
    public void Multiplication_ShouldReturnCorrectProduct()
    {
        // Arrange
        var money = new Money(10m);

        // Act
        var result = money * 3;

        // Assert
        result.Amount.Should().Be(30m);
    }

    [Fact]
    public void Equals_ShouldReturnTrue_WhenAmountsAreEqual()
    {
        // Arrange
        var money1 = new Money(10m);
        var money2 = new Money(10m);

        // Act & Assert
        money1.Equals(money2).Should().BeTrue();
        (money1 == money2).Should().BeFalse(); // Different instances
    }

    [Theory]
    [InlineData(10, 5, true)]
    [InlineData(5, 10, false)]
    [InlineData(10, 10, false)]
    public void ComparisonOperators_ShouldWorkCorrectly(decimal amount1, decimal amount2, bool expectedGreater)
    {
        // Arrange
        var money1 = new Money(amount1);
        var money2 = new Money(amount2);

        // Act & Assert
        (money1 > money2).Should().Be(expectedGreater);
    }
}
