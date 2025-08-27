using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Chat.Dtos;

namespace game_x.application.Features.Chat.Queries.ListUnassignedQueue;

public sealed class ListUnassignedQueueQueryHandler(IConversationRepo conversationRepo)
    : IRequestHandler<ListUnassignedQueueQuery, CursorResult<SupportConversationDto>>
{
    public async Task<CursorResult<SupportConversationDto>> Handle(ListUnassignedQueueQuery request, CancellationToken ct)
    {
        return await conversationRepo.GetUnassignedQueueByCursorAsync(
            limit: request.Limit ?? 20,
            cursor: request.Cursor,
            ct: ct);
    }
}