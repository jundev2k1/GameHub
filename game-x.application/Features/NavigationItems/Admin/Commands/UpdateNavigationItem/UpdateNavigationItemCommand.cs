using System.Text.Json.Serialization;

namespace game_x.application.Features.NavigationItems.Admin.Commands.UpdateNavigationItem;

public record UpdateNavigationItemCommand(
    [property: JsonIgnore] Guid Id,
    string Title,
    string Slug,
    NavigationTargetType TargetType,
    Guid? TargetId,
    string CustomUrl,
    int Priority,
    bool IsActive) : ICommand;
