namespace game_x.application.Events.OnGameRegister;

public record OnGameRegisterEvent(Guid GamePlatformId, string UserId) : IApplicationEvent;
