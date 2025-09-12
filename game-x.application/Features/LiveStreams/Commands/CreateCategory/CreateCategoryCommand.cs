using game_x.application.Features.LiveStreams.Dtos;

namespace game_x.application.Features.LiveStreams.Commands.CreateCategory;

public record CreateCategoryCommand(
    string Name,
    string Description,
    string Note,
    int Priority,
    bool IsActive = true) : ICommand;
