using System.Text.Json.Serialization;

namespace game_x.application.Features.Games.Admin.Commands.UpdateGameTagTranslations;

public record UpdateGameTagTranslationsCommand(
    [property: JsonIgnore] Guid GameTagId,
    GameTagTranslationItem[] Translations) : ICommand;

public record GameTagTranslationItem(
    string LanguageCode,
    string Name,
    string Description,
    string Notes);
