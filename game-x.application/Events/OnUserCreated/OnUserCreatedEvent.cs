namespace game_x.application.Events.OnUserCreated;

public record OnUserCreatedEvent(string Email) : IApplicationEvent;
