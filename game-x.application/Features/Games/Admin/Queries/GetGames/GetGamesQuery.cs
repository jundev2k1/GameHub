using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Features.Games.Dtos;

namespace game_x.application.Features.Games.Admin.Queries.GetGames;

public record GetGamesQuery(
    string? Keyword,
    Guid? PlatformId,
    Guid[]? CategoryIds,
    Guid[]? TypeIds,
    bool? IsActive,
    int PageIndex = 1,
    int PageSize = 20) : IQuery<PaginationResult<GameInfoDto>>;
