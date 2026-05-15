using System.Text.Json.Serialization;

namespace game_x.application.Features.Games.Admin.Commands.UpdateGameTagTranslations;

public record UpdateGameTagTranslationsCommand(
    [property: JsonIgnore] Guid GameTagId,
    GameTypeTranslationItem[] Translations) : ICommand;

public record GameTypeTranslationItem(
    string LanguageCode,
    string Name,
    string Description,
    string Notes);
