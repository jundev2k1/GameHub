using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Features.Chat.Dtos;

namespace game_x.application.Features.Chat.Queries.ListHiddenConversationsForClient;

public record ListHiddenConversationsForClientQuery(
    int? Limit,
    string? Cursor = null) : IQuery<CursorResult<ListedConversationDto>>;