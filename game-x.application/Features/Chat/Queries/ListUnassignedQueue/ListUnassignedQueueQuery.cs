using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Features.Chat.Dtos;

namespace game_x.application.Features.Chat.Queries.ListUnassignedQueue;

public record ListUnassignedQueueQuery(
    int? Limit,
    string? Cursor = null) : IQuery<CursorResult<SupportConversationDto>>;