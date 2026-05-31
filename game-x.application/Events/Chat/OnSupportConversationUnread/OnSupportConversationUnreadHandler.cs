using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Events.Chat.OnSupportConversationUnread;

public sealed class OnSupportConversationUnreadHandler(
    IUnitOfWork unitOfWork,
    IChatHubService chatHubService) : IApplicationEventHandler<OnSupportConversationUnreadEvent>
{
    public async Task Handle(OnSupportConversationUnreadEvent @event, CancellationToken ct = default)
    {
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await chatHubService.SendSupportConversationUnreadAsync(@event.Dto);
        }, ct);
    }
}