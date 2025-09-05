namespace game_x.application.Features.Games.Admin.Commands.CreateGameTag;

public record CreateGameTagCommand(
    string Name,
    string Description,
    string Icon,
    string Color,
    string Note) : ICommand;
