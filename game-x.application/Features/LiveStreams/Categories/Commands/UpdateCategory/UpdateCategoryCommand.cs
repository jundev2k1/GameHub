using System.Text.Json.Serialization;

namespace game_x.application.Features.LiveStreams.Categories.Commands.UpdateCategory;

public record UpdateCategoryCommand(
    [property: JsonIgnore] Guid Id,
    string Name,
    string Description,
    string Note,
    int Priority,
    bool IsActive = true) : ICommand;
