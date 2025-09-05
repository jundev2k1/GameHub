using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Chat.Dtos;

namespace game_x.application.Features.Chat.Queries.ListSupportConversations;

public sealed class ListSupportConversationsHandler(IConversationRepo conversationRepo)
    : IQueryHandler<ListSupportConversationsQuery, CursorResult<SupportConversationDto>>
{
    public async Task<CursorResult<SupportConversationDto>> Handle(ListSupportConversationsQuery request, CancellationToken ct = default)
    {
        return await conversationRepo.GetSupportConversationsAsync(
            limit: request.Limit ?? 20,
            cursor: request.Cursor,
            ct: ct);
    }
}
