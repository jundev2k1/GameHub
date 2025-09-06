namespace game_x.application.Features.Games.Admin.Commands.CreateGameType;

public record CreateGameTypeCommand(
    string Name,
    string Description,
    string Note,
    int Priority) : ICommand;
