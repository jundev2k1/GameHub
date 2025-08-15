namespace game_x.application.Events.OnVerifyUpdated;

public record OnVerifyUpdatedEvent(UserBankAccount UserBankAccount) : IApplicationEvent;