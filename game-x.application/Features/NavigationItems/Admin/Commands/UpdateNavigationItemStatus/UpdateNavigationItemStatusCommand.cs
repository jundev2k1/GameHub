using System.Text.Json.Serialization;

namespace game_x.application.Features.NavigationItems.Admin.Commands.UpdateNavigationItemStatus;

public record UpdateNavigationItemStatusCommand(
    [property: JsonIgnore] Guid Id,
    bool Status) : ICommand;
