using System.Text.Json.Serialization;

namespace game_x.application.Features.Games.Admin.Commands.UpdateGamePlatformTranslations;

public record UpdateGamePlatformTranslationsCommand(
    [property: JsonIgnore] Guid GamePlatformId,
    GamePlatformTranslationItem[] Translations) : ICommand;

public record GamePlatformTranslationItem(
    string LanguageCode,
    string Name,
    string Description,
    string Notes);
