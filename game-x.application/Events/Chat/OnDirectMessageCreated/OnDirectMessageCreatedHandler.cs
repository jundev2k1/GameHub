using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Events.OnDirectMessageCreated;

public sealed class OnDirectMessageCreatedHandler(
    IUnitOfWork unitOfWork,
    IChatHubService chatHubService,
    IConversationMemberRepo convMemberRepo) : IApplicationEventHandler<OnDirectMessageCreatedEvent>
{
    public async Task Handle(OnDirectMessageCreatedEvent @event, CancellationToken ct = default)
    {
        await unitOfWork.WithTransactionAsync(async () =>
        {
            var members = await convMemberRepo.GetMembersByConvIdAsync(@event.Res.Conv.ConversationId, ct);
            await chatHubService.SendDirectMessageAsync(@event.Res, members);
        }, ct);
    }
}