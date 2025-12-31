namespace Payments.Application.Events;

public interface IEvent
{
    DateTime OccurredAt { get; }
}
