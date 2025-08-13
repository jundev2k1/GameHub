namespace game_x.application.Events.OnUserLogin;

public record OnUserLoginEvent(string UserId) : IApplicationEvent;