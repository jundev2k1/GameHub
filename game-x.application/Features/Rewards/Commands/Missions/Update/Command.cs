using game_x.domain.Enum.Rewards;
using game_x.domain.ValueObjects.Missions;
using Newtonsoft.Json;

namespace game_x.application.Features.Rewards.Commands.Missions.Update;

public sealed record UpdateMissionCommand : ICommand<Unit>
{
    [JsonIgnore]
    public Guid Id { get; init; }
    public string? Code { get; init; }
    public string? Title { get; init; }
    public string? Description { get; init; }
    public MissionType? Type { get; init; }
    public bool? IsActive { get; init; }
    public MissionResetType? ResetType { get; init; }
    public UserEventType[]TriggerEvents { get; init; } = [];
    public MissionConfigData? Config { get; init; }
    public DateTime? StartAt { get; init; }
    public DateTime? EndAt { get; init; }
}