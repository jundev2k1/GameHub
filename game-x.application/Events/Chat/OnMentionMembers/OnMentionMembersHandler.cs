using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Events.Chat.OnMentionMembers;

public sealed class OnMentionMembersHandler(
    IUnitOfWork unitOfWork,
    IChatHubService chatHubService,
    IConversationMemberRepo convMemberRepo) : IApplicationEventHandler<OnMentionMembersEvent>
{
    public async Task Handle(OnMentionMembersEvent @event, CancellationToken ct = default)
    {
        await unitOfWork.WithTransactionAsync(async () =>
        {
            var members = await convMemberRepo.GetMembersByConvIdAsync(@event.Res.Conv.ConversationId, ct);
            await chatHubService.SendDirectMessageAsync(@event.Res, members);
        }, ct);
    }
}