using game_x.application.Common.Abstractions.Events;
using MediatR;

namespace game_x.infrastructure.Eventing;

public sealed class ApplicationEventDispatcher(IMediator mediator) : IApplicationEventDispatcher
{
    public Task Publish<TEvent>(TEvent @event, CancellationToken ct = default)
        where TEvent : IApplicationEvent
        => mediator.Publish(@event, ct);
}
