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
    int PageSize = 20) : IQuery<PaginationResult<GameItemDto>>;
