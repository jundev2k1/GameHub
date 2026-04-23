using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Events.Chat.OnClientCountTotalUnread;

public sealed class OnClientCountTotalUnreadHandler(
    IUnitOfWork unitOfWork,
    IChatHubService chatHubService) : IApplicationEventHandler<OnClientCountTotalUnreadEvent>
{
    public async Task Handle(OnClientCountTotalUnreadEvent @event, CancellationToken ct = default)
    {
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await chatHubService.SendTotalUnreadCountAsync(@event.UserId, @event.TotalUnreadCount);
        }, ct);
    }
}