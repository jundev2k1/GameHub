using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Events.OnDeleteMessage;

public sealed class OnDeleteMessageHandler(
    IUnitOfWork unitOfWork,
    IChatHubService chatHubService) : IApplicationEventHandler<OnDeleteMessageEvent>
{
    public async Task Handle(OnDeleteMessageEvent @event, CancellationToken ct = default)
    {
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await chatHubService.SendDeletedMessageAsync(@event.Dto);
        }, ct);
    }
}