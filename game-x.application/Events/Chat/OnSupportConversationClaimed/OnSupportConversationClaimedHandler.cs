using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Events.Chat.OnSupportConversationClaimed;

public sealed class OnSupportConversationClaimedHandler(
    IUnitOfWork unitOfWork,
    IChatHubService chatHubService) : IApplicationEventHandler<OnSupportConversationClaimedEvent>
{
    public async Task Handle(OnSupportConversationClaimedEvent @event, CancellationToken ct = default)
    {
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await chatHubService.SendSupportConversationClaimedAsync(@event.Dto);
        }, ct);
    }
}