using game_x.domain.Entities.Rewards;
using game_x.domain.Enum.Rewards;

namespace game_x.application.Features.Rewards.Strategies.Missions;

public interface IMissionProgressStrategy
{
    UserEventType SupportedType { get; }

    Task ProcessAsync(Mission mission, UserMission userMission, UserEvent userEvent, CancellationToken ct = default);
}