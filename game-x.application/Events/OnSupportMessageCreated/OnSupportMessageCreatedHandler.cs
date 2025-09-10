using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Events.OnSupportMessageCreated;

public sealed class OnSupportMessageCreatedHandler(
    IUnitOfWork unitOfWork,
    IChatHubService chatHubService) : IApplicationEventHandler<OnSupportMessageCreatedEvent>
{
    public async Task Handle(OnSupportMessageCreatedEvent @event, CancellationToken ct = default)
    {
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await chatHubService.SendSupportMessageAsync(@event.Res);
        }, ct);
    }
}