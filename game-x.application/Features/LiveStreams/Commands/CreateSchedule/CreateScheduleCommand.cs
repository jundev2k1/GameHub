using game_x.application.Features.LiveStreams.Dtos;

namespace game_x.application.Features.LiveStreams.Commands.CreateSchedule;

public record CreateScheduleCommand(
    string Title,
    string Description,
    string Note,
    DateTime StartTime,
    DateTime EndTime,
    ScheduleCategoryDto[] Categories,
    string? TalentId) : ICommand;

