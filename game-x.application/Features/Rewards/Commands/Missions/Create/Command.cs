using game_x.domain.Enum.Rewards;
using game_x.domain.ValueObjects.Missions;

namespace game_x.application.Features.Rewards.Commands.Missions.Create;

public sealed record CreateMissionCommand: ICommand<Unit>
{
    public string Code { get; init; } = String.Empty;
    public MissionType Type { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public MissionResetType? ResetType { get; init; }
    public MissionConfigData ConfigData { get; init; } = MissionConfigData.Default();
    public DateTime? StartAt { get; init; }
    public DateTime? EndAt { get; init; }
}