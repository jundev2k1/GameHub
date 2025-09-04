using System.Text.Json.Serialization;

namespace game_x.application.Features.Games.Admin.Commands.UpdateGameTag;

public record UpdateGameTagCommand(
    [property: JsonIgnore] Guid Id,
    string Name,
    string Description,
    string Icon,
    string Color,
    string Note) : ICommand;
