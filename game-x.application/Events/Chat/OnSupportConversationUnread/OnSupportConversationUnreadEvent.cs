using game_x.application.Features.Chat.Dtos;

namespace game_x.application.Events.Chat.OnSupportConversationUnread;

public record OnSupportConversationUnreadEvent(IReadOnlyCollection<ConversationUnreadDto> Dto) : IApplicationEvent;