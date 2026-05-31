namespace game_x.application.Events.Rewards.OnDailyCheckIn;

public sealed record OnDailyCheckInEvent(string UserId) : IApplicationEvent;