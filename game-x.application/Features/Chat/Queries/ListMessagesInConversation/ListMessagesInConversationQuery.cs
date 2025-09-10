using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;

namespace game_x.application.Features.Chat.Queries.ListMessagesInConversation;

public record ListMessagesInConversationQuery(
    Guid ConvId,
    int? Limit,
    string? ActorId = null,
    string? Cursor = null) : IQuery<CursorResult<ListMessageDto>>;