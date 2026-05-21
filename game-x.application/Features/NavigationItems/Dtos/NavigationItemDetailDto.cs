namespace game_x.application.Features.NavigationItems.Dtos;

public sealed class NavigationItemDetailDto : NavigationItemDto
{
    public NavigationItemTranslationInfo[] Translations => [.. NavigationTranslations.Values];
}
