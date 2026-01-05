using Payments.Application.Events;

namespace Payments.Tests.Application;

public class PaymentApprovedEventTests
{
    [Fact]
    public void When_CreatingEvent_WithValidData_Expect_PropertiesSet()
    {
        // Arrange
        long paymentId = 12345;
        string orderId = "order-123";
        decimal amount = 100.50m;
        DateTime before = DateTime.UtcNow;

        // Act
        PaymentApprovedEvent @event = new(paymentId, orderId, amount);

        // Assert
        Assert.Equal(paymentId, @event.PaymentId);
        Assert.Equal(orderId, @event.OrderId);
        Assert.Equal(amount, @event.Amount);
        Assert.True(@event.OccurredAt >= before);
        Assert.True(@event.OccurredAt <= DateTime.UtcNow);
    }

    [Fact]
    public void When_CreatingEvent_Expect_OccurredAtSetToCurrentTime()
    {
        // Arrange
        DateTime before = DateTime.UtcNow;

        // Act
        PaymentApprovedEvent @event = new(1, "order-123", 100m);

        // Assert
        DateTime after = DateTime.UtcNow;
        Assert.InRange(@event.OccurredAt, before, after);
    }
}
