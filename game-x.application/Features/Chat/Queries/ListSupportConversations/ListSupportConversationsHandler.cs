using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Contract.Persistence.Identity;
using game_x.application.Features.Chat.Dtos;

namespace game_x.application.Features.Chat.Queries.ListSupportConversations;

public sealed class ListSupportConversationsHandler(IConversationService conversationService)
    : IQueryHandler<ListSupportConversationsQuery, CursorResult<SupportConversationDto>>
{
    public async Task<CursorResult<SupportConversationDto>> Handle(ListSupportConversationsQuery request, CancellationToken ct = default)
    {
        return await conversationService.GetSupportConversationsAsync(
            limit: request.Limit ?? 20,
            cursor: request.Cursor,
            ct: ct);
    }
}
