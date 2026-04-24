using game_x.application.Contract.Infrastructure.SignalR.Dtos.Friend;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Events.Friendship.OnFriendBlocked;

public sealed class OnFriendBlockedHandler(
    IUnitOfWork unitOfWork,
    IChatHubService chatHubService
    )
    : IApplicationEventHandler<OnFriendBlockedEvent>
{
    public async Task Handle(OnFriendBlockedEvent @event, CancellationToken ct = default)
    {
        var dto = @event.Dto.Adapt<FriendBlockedSignalDto>();
        await unitOfWork.WithTransactionAsync(async () =>
        {
            if(dto.BlockedUserId is not null)
                await chatHubService.SendFriendBlockedAsync(dto);
        }, ct);
    }
}