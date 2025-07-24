namespace game_x.application.Common.Abstractions.Events;

public interface IApplicationEventDispatcher
{
    Task Publish<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IApplicationEvent;
}
