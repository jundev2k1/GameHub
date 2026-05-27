using System.Text.Json.Serialization;

namespace game_x.application.Features.Games.Admin.Commands.UpsertGameMedias;

public record UpsertGameMediasCommand(
    [property: JsonIgnore] Guid Id,
    GameMediaItemInfo[] Items) : ICommand;

public record GameMediaItemInfo(
    Guid? Id,
    GameMediaType Type,
    GameMediaCategory Category,
    string Title,
    string Note,
    int Priority);
