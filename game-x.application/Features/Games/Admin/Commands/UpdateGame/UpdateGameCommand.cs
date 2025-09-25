using game_x.application.Common.Files;
using System.Text.Json.Serialization;

namespace game_x.application.Features.Games.Admin.Commands.UpdateGame;

public record UpdateGameCommand(
    [property: JsonIgnore] Guid Id,
    string Name,
    string Description,
    string Note,
    int Priority,
    bool IsActive,
    FileUpload? Thumbnail,
    GameCategoryItem[]? Categories,
    GameTypeItem[]? Types,
    GameTagItem[]? Tags) : ICommand;

public record GameTagItem(
    Guid Id,
    bool IsPrimary,
    int Priority);

public record GameCategoryItem(
    Guid Id,
    bool IsPrimary,
    int Priority);

public record GameTypeItem(
    Guid Id,
    bool IsPrimary,
    int Priority);