namespace game_x.application.Common.Abstractions.Events;

public interface IApplicationEventHandler<in TEvent> : INotificationHandler<TEvent>
    where TEvent : IApplicationEvent
{
}
