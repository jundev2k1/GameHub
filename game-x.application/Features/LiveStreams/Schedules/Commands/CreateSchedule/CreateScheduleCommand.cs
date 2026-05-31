using game_x.application.Features.LiveStreams.Schedules.Dtos;

namespace game_x.application.Features.LiveStreams.Schedules.Commands.CreateSchedule;

public record CreateScheduleCommand(
    string Title,
    string Description,
    string Note,
    DateTime StartTime,
    DateTime EndTime,
    LiveStreamScheduleCategoryDto[] Categories,
    string? TalentId) : ICommand;

