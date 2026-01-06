using Payments.Domain.ValueObjects;

namespace Payments.Tests.Domain;

public class MoneyTests
{
    [Fact]
    public void When_CreatingMoney_WithValidAmount_Expect_Success()
    {
        // Arrange
        decimal amount = 100.50m;

        // Act
        Money money = Money.Create(amount);

        // Assert
        Assert.Equal(100.50m, money.Amount);
    }

    [Fact]
    public void When_CreatingMoney_WithNegativeAmount_Expect_ArgumentException()
    {
        // Arrange
        decimal amount = -10m;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => Money.Create(amount));
    }

    [Fact]
    public void When_CreatingMoney_WithMoreThanTwoDecimals_Expect_RoundedValue()
    {
        // Arrange
        decimal amount = 100.555m;

        // Act
        Money money = Money.Create(amount);

        // Assert
        Assert.Equal(100.56m, money.Amount);
    }

    [Fact]
    public void When_CreatingMoney_WithZeroAmount_Expect_Success()
    {
        // Arrange
        decimal amount = 0m;

        // Act
        Money money = Money.Create(amount);

        // Assert
        Assert.Equal(0m, money.Amount);
    }

    [Fact]
    public void When_ComparingMoney_WithSameAmount_Expect_Equal()
    {
        // Arrange
        Money money1 = Money.Create(100m);
        Money money2 = Money.Create(100m);

        // Act & Assert
        Assert.Equal(money1, money2);
        Assert.True(money1 == money2);
    }

    [Fact]
    public void When_ComparingMoney_WithDifferentAmount_Expect_NotEqual()
    {
        // Arrange
        Money money1 = Money.Create(100m);
        Money money2 = Money.Create(200m);

        // Act & Assert
        Assert.NotEqual(money1, money2);
        Assert.True(money1 != money2);
    }

    [Fact]
    public void When_ComparingMoney_WithNull_Expect_NotEqual()
    {
        // Arrange
        Money money = Money.Create(100m);

        // Act & Assert
        Assert.False(money.Equals(null));
    }

    [Fact]
    public void When_GettingHashCode_WithSameAmount_Expect_SameHashCode()
    {
        // Arrange
        Money money1 = Money.Create(100m);
        Money money2 = Money.Create(100m);

        // Act & Assert
        Assert.Equal(money1.GetHashCode(), money2.GetHashCode());
    }
}
