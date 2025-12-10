namespace StackFood.Products.Domain.ValueObjects;

public class Money : IEquatable<Money>
{
    public decimal Amount { get; private set; }

    private Money() { }

    public Money(decimal amount)
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative", nameof(amount));

        Amount = decimal.Round(amount, 2);
    }

    public static Money operator +(Money left, Money right)
    {
        return new Money(left.Amount + right.Amount);
    }

    public static Money operator -(Money left, Money right)
    {
        return new Money(left.Amount - right.Amount);
    }

    public static Money operator *(Money money, decimal multiplier)
    {
        return new Money(money.Amount * multiplier);
    }

    public static bool operator >(Money left, Money right)
    {
        return left.Amount > right.Amount;
    }

    public static bool operator <(Money left, Money right)
    {
        return left.Amount < right.Amount;
    }

    public static bool operator >=(Money left, Money right)
    {
        return left.Amount >= right.Amount;
    }

    public static bool operator <=(Money left, Money right)
    {
        return left.Amount <= right.Amount;
    }

    public bool Equals(Money? other)
    {
        if (other is null) return false;
        return Amount == other.Amount;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Money);
    }

    public override int GetHashCode()
    {
        return Amount.GetHashCode();
    }

    public override string ToString()
    {
        return Amount.ToString("F2");
    }

    public static implicit operator decimal(Money money)
    {
        return money.Amount;
    }

    public static explicit operator Money(decimal amount)
    {
        return new Money(amount);
    }
}
