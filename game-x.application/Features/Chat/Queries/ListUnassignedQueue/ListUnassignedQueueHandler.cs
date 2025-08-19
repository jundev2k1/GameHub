using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Chat.Dtos;

namespace game_x.application.Features.Chat.Queries.ListUnassignedQueue;

public sealed class ListUnassignedQueueQueryHandler(IConversationRepo conversationRepo)
    : IRequestHandler<ListUnassignedQueueQuery, CursorResult<ConversationQueueItemDto>>
{
    public async Task<CursorResult<ConversationQueueItemDto>> Handle(ListUnassignedQueueQuery request, CancellationToken ct)
    {
        // Prefer cursor; ignore page index/size for this endpoint
        var limit = Math.Clamp(request.Limit ?? 20, 1, 100);

        return await conversationRepo.GetUnassignedQueueByCursorAsync(
            limit: limit,
            cursor: request.Cursor,
            q: request.Q,
            search: request.Search,
            ct: ct);
    }
}