using System.Text.Json.Serialization;

namespace game_x.application.Features.Games.Admin.Commands.UpdateGameTranslations;

public record UpdateGameTranslationsCommand(
    [property: JsonIgnore] Guid GameId,
    GameTranslationItem[] Translations) : ICommand;

public record GameTranslationItem(
    string LanguageCode,
    string Name,
    string Description,
    string Notes);