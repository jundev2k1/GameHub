using game_x.application.Features.LiveStreams.Dtos;
using System.Text.Json.Serialization;

namespace game_x.application.Features.LiveStreams.Commands.UpdateSchedule;

public record UpdateScheduleCommand(
    [property: JsonIgnore] Guid Id,
    string? Title,
    string? Description,
    string? Note,
    DateTime? StartTime,
    DateTime? EndTime,
    ScheduleCategoryDto[]? Categories) : ICommand;
