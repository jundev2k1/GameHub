using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Events.Rewards.OnUserInventoryUpdated;

public sealed class OnUserInventoryUpdatedHandler(
    IUnitOfWork unitOfWork,
    IClientHubService clientHubService) : IApplicationEventHandler<OnUserInventoryUpdatedEvent>
{
    public async Task Handle(OnUserInventoryUpdatedEvent @event, CancellationToken ct = default)
    {
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await clientHubService.SendInventoryAsync(@event.UserId, @event.Dto);
        }, ct);
    }
}
