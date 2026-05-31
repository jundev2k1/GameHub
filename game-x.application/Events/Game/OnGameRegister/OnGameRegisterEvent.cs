namespace game_x.application.Events.Game.OnGameRegister;

public record OnGameRegisterEvent(Guid GamePlatformId, string UserId) : IApplicationEvent;