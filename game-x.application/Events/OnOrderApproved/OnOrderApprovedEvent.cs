namespace game_x.application.Events.OnOrderApproved;

public record OnOrderApprovedEvent(Order Order) : IApplicationEvent;
