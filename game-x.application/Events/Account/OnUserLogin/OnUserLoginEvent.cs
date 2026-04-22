namespace game_x.application.Events.Account.OnUserLogin;

public record OnUserLoginEvent(string UserId) : IApplicationEvent;
