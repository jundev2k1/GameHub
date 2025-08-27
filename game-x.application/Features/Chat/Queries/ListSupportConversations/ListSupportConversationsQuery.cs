using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Features.Chat.Dtos;

namespace game_x.application.Features.Chat.Queries.ListSupportConversations;

public record ListSupportConversationsQuery(int? Limit, string? Cursor = null) : IQuery<CursorResult<SupportConversationDto>>;