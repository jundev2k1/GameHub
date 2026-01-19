using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Features.Chat.Dtos;

namespace game_x.application.Features.Chat.Queries.ListMyConversationsForClient;

public record ListMyConversationsForClientQuery(
    int? Limit,
    string? Cursor = null,
    ConversationType? Type = null) : IQuery<CursorResult<ListedConversationDto>>;