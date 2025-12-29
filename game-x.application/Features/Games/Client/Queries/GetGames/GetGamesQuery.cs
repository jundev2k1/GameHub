using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Features.Games.Dtos;

namespace game_x.application.Features.Games.Client.Queries.GetGames;

public record GetGamesQuery(
    string? Keyword,
    Guid? Platform,
    Guid[]? Categories,
    Guid[]? GameTypes,
    Guid[]? GameTags,
    int PageIndex = 1,
    int PageSize = 20) : IQuery<PaginationResult<GetGamesItemDto>>;

public record GetGamesCategoryItemDto(Guid Id, string Name, bool IsPrimary);

public record GetGamesTypeItemDto(Guid Id, string Name, bool IsPrimary);

public record GetGamesTagItemDto(Guid Id, string Name, string Icon, string Color, bool IsPrimary);

public record GetGamesItemDto(
    Guid Id,
    string GameCode,
    string Name,
    string Description,
    Guid PlatformId,
    string PlatformName,
    string? GameThumbnailUrl,
    GetGamesCategoryItemDto[] Categories,
    GetGamesTypeItemDto[] GameTypes,
    GetGamesTagItemDto[] Tags)
{
    public GetGamesItemDto(GameInfoDto game)
        : this(
            game.Id,
            game.GameCode,
            game.Name,
            game.Description,
            game.PlatformId,
            game.PlatformName,
            game.Thumbnail?.Url,
            [],
            [],
            [])
    {
        Categories = [.. game.Categories
            .OrderByDescending(cate => cate.IsPrimary)
            .ThenByDescending(cate => cate.Priority)
            .Select(cate => new GetGamesCategoryItemDto(cate.Id, cate.Name, cate.IsPrimary))];

        GameTypes = [.. game.GameTypes
            .OrderByDescending(type => type.IsPrimary)
            .ThenByDescending(type => type.Priority)
            .Select(type => new GetGamesTypeItemDto(type.Id, type.Name, type.IsPrimary))];

        Tags = [.. game.GameTags
            .OrderByDescending(tag => tag.IsPrimary)
            .ThenByDescending(tag => tag.Priority)
            .Select(tag => new GetGamesTagItemDto(tag.Id, tag.Name, tag.Icon, tag.Color, tag.IsPrimary))];
    }
}
