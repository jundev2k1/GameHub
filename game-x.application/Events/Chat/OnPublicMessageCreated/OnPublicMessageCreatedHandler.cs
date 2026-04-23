using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Events.Chat.OnPublicMessageCreated;

public sealed class OnPublicMessageCreatedHandler(
    IUnitOfWork unitOfWork,
    IChatHubService chatHubService) : IApplicationEventHandler<OnPublicMessageCreatedEvent>
{
    public async Task Handle(OnPublicMessageCreatedEvent @event, CancellationToken ct = default)
    {
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await chatHubService.SendPublicMessageAsync(@event.Res);
        }, ct);
    }
}