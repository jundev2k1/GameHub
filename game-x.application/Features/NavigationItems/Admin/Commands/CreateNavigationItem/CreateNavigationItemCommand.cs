namespace game_x.application.Features.NavigationItems.Admin.Commands.CreateNavigationItem;

public record CreateNavigationItemCommand(
    string Title,
    string Slug,
    NavigationTargetType TargetType,
    Guid? TargetId,
    string CustomUrl,
    int Priority) : ICommand;
