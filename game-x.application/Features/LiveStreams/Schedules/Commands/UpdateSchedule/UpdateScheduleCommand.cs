using game_x.application.Features.LiveStreams.Schedules.Dtos;
using System.Text.Json.Serialization;

namespace game_x.application.Features.LiveStreams.Schedules.Commands.UpdateSchedule;

public record UpdateScheduleCommand(
    [property: JsonIgnore] Guid Id,
    string? Title,
    string? Description,
    string? Notes,
    DateTime? StartTime,
    DateTime? EndTime,
    LiveStreamScheduleCategoryDto[]? Categories) : ICommand;
