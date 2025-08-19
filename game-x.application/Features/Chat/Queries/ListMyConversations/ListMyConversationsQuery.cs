using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Features.Chat.Dtos;

namespace game_x.application.Features.Chat.Queries.ListMyConversations;

public record ListMyConversationsQuery(
    IEnumerable<QueryFilter> Filters,
    int? Limit,
    string? Q = null,
    string? Search = null,
    string? Cursor = null) : IQuery<CursorResult<ConversationQueueItemDto>>;