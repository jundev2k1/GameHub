using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Events.OnSupportMessageCreatedV2;

public sealed class OnSupportMessageCreatedV2Handler(
    IUnitOfWork unitOfWork,
    IChatHubService chatHubService) : IApplicationEventHandler<OnSupportMessageCreatedV2Event>
{
    public async Task Handle(OnSupportMessageCreatedV2Event @event, CancellationToken ct = default)
    {
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await chatHubService.SendSupportMessageV2Async(@event.Res);
        }, ct);
    }
}