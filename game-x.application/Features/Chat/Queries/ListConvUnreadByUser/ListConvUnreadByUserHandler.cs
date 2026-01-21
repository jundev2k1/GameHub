using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Chat.Dtos;

namespace game_x.application.Features.Chat.Queries.ListConvUnreadByUser;

public sealed class ListConvByUserHandler(
    IUserAccessor userAccessor,
    IConversationMemberRepo convMemberRepo)
    : IRequestHandler<ListConvByUserQuery, IReadOnlyCollection<ConversationUnreadDto>>
{
    public async Task<IReadOnlyCollection<ConversationUnreadDto>> Handle(ListConvByUserQuery request, CancellationToken ct)
    {
        string me = userAccessor.GetUserId();
        return await convMemberRepo.GetTotalUnreadByUserIdAsync(me, request.Type, ct);
    }
}