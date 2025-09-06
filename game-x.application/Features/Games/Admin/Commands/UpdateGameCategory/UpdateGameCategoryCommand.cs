using System.Text.Json.Serialization;

namespace game_x.application.Features.Games.Admin.Commands.UpdateGameCategory;

public record UpdateGameCategoryCommand(
    [property: JsonIgnore] Guid Id,
    string Name,
    string Description,
    string Note,
    int Priority,
    bool IsActive) : ICommand;
