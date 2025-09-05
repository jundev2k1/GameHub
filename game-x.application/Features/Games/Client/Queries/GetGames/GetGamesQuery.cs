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
    GetGamesCategoryItemDto[] Categories,
    GetGamesTypeItemDto[] GameTypes,
    GetGamesTagItemDto[] Tags)
{
    public GetGamesItemDto(GameInfoDto game)
        : this(game.Id, game.GameCode, game.Name, game.Description, game.PlatformId, game.PlatformName, [], [], [])
    {
        Categories = [.. game.Categories
            .OrderByDescending(cate => cate.IsPrimary)
            .ThenBy(cate => cate.Priority)
            .Select(cate => new GetGamesCategoryItemDto(cate.Id, cate.Name, cate.IsPrimary))];

        GameTypes = [.. game.GameTypes
            .OrderByDescending(cate => cate.IsPrimary)
            .ThenBy(cate => cate.Priority)
            .Select(type => new GetGamesTypeItemDto(type.Id, type.Name, type.IsPrimary))];

        Tags = [.. game.GameTags
            .OrderByDescending(cate => cate.IsPrimary)
            .ThenBy(cate => cate.Priority)
            .Select(tag => new GetGamesTagItemDto(tag.Id, tag.Name, tag.Icon, tag.Color, tag.IsPrimary))];
    }
}
