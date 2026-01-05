using game_x.application.Features.Chat.Dtos;

namespace game_x.application.Features.Chat.Queries.ListSupportConvUnread;

public record ListSupportConvUnreadQuery : IQuery<IReadOnlyCollection<ConversationUnreadDto>>;