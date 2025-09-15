using System.Text.Json.Serialization;

namespace game_x.application.Features.LiveStreams.Commands.CancelSchedule;

public record CancelScheduleCommand(
    [property: JsonIgnore] Guid Id,
    string Reason) : ICommand;
