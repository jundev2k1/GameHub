namespace game_x.application.Events.Account.OnUserCreated;

public record OnUserCreatedEvent(string Email) : IApplicationEvent;
