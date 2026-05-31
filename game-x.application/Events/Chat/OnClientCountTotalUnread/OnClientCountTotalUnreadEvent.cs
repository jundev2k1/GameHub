namespace game_x.application.Events.Chat.OnClientCountTotalUnread;

public record OnClientCountTotalUnreadEvent(string UserId, int TotalUnreadCount) : IApplicationEvent;