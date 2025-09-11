using System.Text.Json.Serialization;

namespace game_x.application.Features.LiveStreams.Commands.UpdateSchedule;

public record UpdateScheduleCommand(
    [property: JsonIgnore] Guid Id) : ICommand;
