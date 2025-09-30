using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Events.OnMarkMessageAsRead;

public sealed class OnMarkMessageAsReadHandler(
    IUnitOfWork unitOfWork,
    IChatHubService chatHubService) : IApplicationEventHandler<OnMarkMessageAsReadEvent>
{
    public async Task Handle(OnMarkMessageAsReadEvent @event, CancellationToken ct = default)
    {
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await chatHubService.SendMarkAsReadAsync(@event.Dto, @event.UserId);
        }, ct);
    }
}