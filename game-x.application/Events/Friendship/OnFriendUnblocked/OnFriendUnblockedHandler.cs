using game_x.application.Contract.Infrastructure.SignalR.Dtos.Friend;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Events.Friendship.OnFriendUnblocked;

public sealed class OnFriendUnblockedHandler(
    IUnitOfWork unitOfWork,
    IChatHubService chatHubService
    )
    : IApplicationEventHandler<OnFriendUnblockedEvent>
{
    public async Task Handle(OnFriendUnblockedEvent @event, CancellationToken ct = default)
    {
        var dto = @event.Dto.Adapt<FriendBlockedSignalDto>();
        await unitOfWork.WithTransactionAsync(async () =>
        {
            if(dto.BlockedUserId is not null)
                await chatHubService.SendFriendUnblockedAsync(dto);
        }, ct);
    }
}