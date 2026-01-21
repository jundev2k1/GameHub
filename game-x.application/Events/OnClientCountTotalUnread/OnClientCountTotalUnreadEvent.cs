namespace game_x.application.Events.OnClientCountTotalUnread;

public record OnClientCountTotalUnreadEvent(string UserId, int TotalUnreadCount) : IApplicationEvent;