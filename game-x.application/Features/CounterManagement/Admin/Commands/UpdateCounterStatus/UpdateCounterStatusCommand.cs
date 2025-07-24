using System.Text.Json.Serialization;

namespace game_x.application.Features.CounterManagement.Admin.Commands.UpdateCounterStatus;

public record UpdateCounterStatusCommand : ICommand
{
    [JsonIgnore]
    public Guid CounterId { get; init; }
    public CounterStatus Status { get; init; }
}
