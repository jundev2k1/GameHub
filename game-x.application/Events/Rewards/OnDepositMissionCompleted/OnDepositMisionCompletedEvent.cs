using game_x.application.Contract.Infrastructure.SignalR.Dtos.Rewards;

namespace game_x.application.Events.Rewards.OnDepositMissionCompleted;

public sealed record OnDepositMissionCompletedEvent(string UserId, MissionSignalDto Dto) : IApplicationEvent;