using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Features.Chat.Dtos;

namespace game_x.application.Features.Chat.Queries.ListMessagesInConversation;

public record ListMessagesInConversationQuery(
    Guid ConvId,
    int? Limit,
    string? ActorId = null,
    string? Cursor = null) : IQuery<CursorResult<ListedMessageDto>>;