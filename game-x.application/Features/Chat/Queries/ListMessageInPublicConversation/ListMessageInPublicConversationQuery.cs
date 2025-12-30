using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Features.Chat.Dtos;

namespace game_x.application.Features.Chat.Queries.ListMessageInPublicConversation;

public record ListMessageInPublicConversationQuery(int? Limit, string? Cursor = null) : IQuery<CursorResult<ListedMessageDto>>;