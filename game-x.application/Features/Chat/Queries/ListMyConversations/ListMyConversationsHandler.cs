using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Chat.Dtos;

namespace game_x.application.Features.Chat.Queries.ListMyConversations;

public sealed class ListMyConversationsHandler(IUserAccessor userAccessor, IConversationRepo conversationRepo)
    : IRequestHandler<ListMyConversationsQuery, CursorResult<ConversationQueueItemDto>>
{
    public async Task<CursorResult<ConversationQueueItemDto>> Handle(ListMyConversationsQuery request, CancellationToken ct)
    {
        var userId = userAccessor.GetUserId();
        var limit = Math.Clamp(request.Limit ?? 20, 1, 100);

        return await conversationRepo.GetMyConversationsByCursorAsync(
            userId: userId,
            limit: limit,
            cursor: request.Cursor,
            q: request.Q,
            search: request.Search,
            ct: ct);
    }
}