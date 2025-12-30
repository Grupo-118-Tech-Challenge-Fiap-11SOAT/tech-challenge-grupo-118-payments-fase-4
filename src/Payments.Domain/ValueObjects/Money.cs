namespace Payments.Domain.ValueObjects;

public sealed class Money : IEquatable<Money>
{
    public decimal Amount { get; }

    private Money(decimal amount)
    {
        Amount = amount;
    }

    public static Money Create(decimal amount)
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative", nameof(amount));

        return new Money(Math.Round(amount, 2));
    }

    public bool Equals(Money? other)
    {
        if (other is null) return false;
        return Amount == other.Amount;
    }

    public override bool Equals(object? obj) => Equals(obj as Money);

    public override int GetHashCode() => Amount.GetHashCode();

    public static bool operator ==(Money? left, Money? right) => Equals(left, right);

    public static bool operator !=(Money? left, Money? right) => !Equals(left, right);
}
