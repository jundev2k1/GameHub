using game_x.application.Features.Chat.Dtos;

namespace game_x.application.Features.Chat.Queries.ListConvUnreadByUser;

public record ListConvByUserQuery(ConversationType? Type) : IQuery<IReadOnlyCollection<ConversationUnreadDto>>;