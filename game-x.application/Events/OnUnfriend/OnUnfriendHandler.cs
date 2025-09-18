using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Events.OnUnfriend;

public sealed class OnUnfriendHandler(
    IUnitOfWork unitOfWork,
    IChatHubService chatHubService
    )
    : IApplicationEventHandler<OnUnfriendEvent>
{
    public async Task Handle(OnUnfriendEvent @event, CancellationToken ct = default)
    {
        await unitOfWork.WithTransactionAsync(async () =>
        {
            if (@event.Dto.UnfriendedUserId is not null)
                await chatHubService.SendUnfriendAsync(@event.Dto);
        }, ct);
    }
}