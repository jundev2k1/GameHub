namespace game_x.application.Features.Games.Admin.Commands.CreateGameCategory;

public record CreateGameCategoryCommand(
    string Name,
    string Description,
    string Note,
    int Priority) : ICommand;
