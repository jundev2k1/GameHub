using System.Text.Json.Serialization;

namespace game_x.application.Features.CounterManagement.Admin.Commands.UpdateCounter;

public record UpdateCounterCommand : ICommand
{
    [JsonIgnore]
    public Guid CounterId { get; init; }
    public string CounterName { get; init; } = string.Empty;
    public CounterStatus Status { get; init; }
    public string Location { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}
