using game_x.application.Common.Files;
using System.Text.Json.Serialization;

namespace game_x.application.Features.NavigationItems.Admin.Commands.UpdateNavigationItemIcon;

public record UpdateNavigationItemIconCommand(
    [property: JsonIgnore] Guid Id,
    FileUpload File) : ICommand<string>;
