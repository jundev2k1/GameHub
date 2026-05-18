using System.Text.Json.Serialization;

namespace game_x.application.Features.Games.Admin.Commands.UpdateGameCategoryTranslations;

public record UpdateGameCategoryTranslationsCommand(
    [property: JsonIgnore] Guid GameCateId,
    GameCategoryTranslationItem[] Translations) : ICommand;

public record GameCategoryTranslationItem(
    string LanguageCode,
    string Name,
    string Description,
    string Notes);
