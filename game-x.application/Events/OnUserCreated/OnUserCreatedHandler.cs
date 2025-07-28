using game_x.application.Contract.Infrastructure.Services.EmailProcessor;

namespace game_x.application.Events.OnUserCreated;

public sealed class OnUserCreatedHandler(IEmailVerificationProcessor emailVerification)
    : IApplicationEventHandler<OnUserCreatedEvent>
{
    public async Task Handle(OnUserCreatedEvent @event, CancellationToken ct = default)
    {
        emailVerification.SendVerificationEmail(@event.Email);
        await Task.CompletedTask;
    }
}
