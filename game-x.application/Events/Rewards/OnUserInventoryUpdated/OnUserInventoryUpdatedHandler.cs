using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Rewards;

namespace game_x.application.Events.Rewards.OnUserInventoryUpdated;

public sealed class OnUserInventoryUpdatedHandler(
    IUnitOfWork unitOfWork,
    IClientHubService clientHubService) : IApplicationEventHandler<OnUserInventoryUpdatedEvent>
{
    public async Task Handle(OnUserInventoryUpdatedEvent @event, CancellationToken ct = default)
    {
        var signalDto =  @event.Dto
            .Select(x => x.Adapt<UserInventorySignalDto>())
            .ToArray();
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await clientHubService.SendInventoryAsync(@event.UserId, signalDto);
        }, ct);
    }
}
