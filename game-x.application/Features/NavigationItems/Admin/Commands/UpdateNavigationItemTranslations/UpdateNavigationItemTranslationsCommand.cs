using System.Text.Json.Serialization;

namespace game_x.application.Features.NavigationItems.Admin.Commands.UpdateNavigationItemTranslations;

public record UpdateNavigationItemTranslationsCommand(
    [property: JsonIgnore] Guid Id,
    NavigationTranslationItem[] Translations) : ICommand;

public record NavigationTranslationItem(
    string LanguageCode,
    string Title);
