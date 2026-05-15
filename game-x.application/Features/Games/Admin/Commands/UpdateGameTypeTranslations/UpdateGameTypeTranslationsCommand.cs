using System.Text.Json.Serialization;

namespace game_x.application.Features.Games.Admin.Commands.UpdateGameTypeTranslations;

public record UpdateGameTypeTranslationsCommand(
    [property: JsonIgnore] Guid GameTypeId,
    GameTypeTranslationItem[] Translations) : ICommand;

public record GameTypeTranslationItem(
    string LanguageCode,
    string Name,
    string Description,
    string Notes);
