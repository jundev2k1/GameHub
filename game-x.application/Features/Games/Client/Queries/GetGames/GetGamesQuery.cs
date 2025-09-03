using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Features.Games.Dtos;

namespace game_x.application.Features.Games.Client.Queries.GetGames;

public record GetGamesQuery(
    string? Keyword,
    Guid? Platform,
    Guid[]? Categories,
    Guid[]? GameTypes,
    int PageIndex = 1,
    int PageSize = 20) : IQuery<PaginationResult<GetGamesItemDto>>;

public record GetGamesItemDto(
    Guid Id,
    string GameCode,
    string Name,
    string Description,
    string Note,
    Guid PlatformId,
    string PlatformName,
    GameCategoryInfo[] Categories,
    GameTypeInfo[] GameTypes);
