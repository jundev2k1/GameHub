using System.Text.Json.Serialization;

namespace game_x.application.Features.Games.Admin.Commands.UpdateGamePlatform;

public record UpdateGamePlatformCommand(
    [property: JsonIgnore] Guid Id,
    string Name,
    string Description,
    string Note,
    int Priority,
    bool IsActive) : ICommand;
