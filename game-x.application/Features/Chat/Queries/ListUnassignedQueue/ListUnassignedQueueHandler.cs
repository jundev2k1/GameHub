using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Contract.Persistence.Identity;
using game_x.application.Features.Chat.Dtos;

namespace game_x.application.Features.Chat.Queries.ListUnassignedQueue;

public sealed class ListUnassignedQueueQueryHandler(IConversationService conversationService)
    : IRequestHandler<ListUnassignedQueueQuery, CursorResult<SupportConversationDto>>
{
    public async Task<CursorResult<SupportConversationDto>> Handle(ListUnassignedQueueQuery request, CancellationToken ct)
    {
        return await conversationService.GetUnassignedQueueByCursorAsync(
            limit: request.Limit ?? 20,
            cursor: request.Cursor,
            ct: ct);
    }
}