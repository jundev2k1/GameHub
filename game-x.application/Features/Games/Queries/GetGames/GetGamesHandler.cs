using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Games.Dtos;
using System.Linq.Expressions;

namespace game_x.application.Features.Games.Queries.GetGames;

public sealed class GetGamesHandler(IGameRepo gameRepo) : IQueryHandler<GetGamesQuery, PaginationResult<GameInfoDto>>
{
    public async Task<PaginationResult<GameInfoDto>> Handle(GetGamesQuery request, CancellationToken ct = default)
    {
        var result = await gameRepo.GetsByCriteriaAsync(
            GetFilterCondition(request),
            request.PageIndex,
            request.PageSize,
            ct);
        return new PaginationResult<GameInfoDto>(
            [.. result.Items.Select(i => i.Adapt<GameInfoDto>())],
            result.TotalItems,
            result.TotalPages,
            result.PageNumber,
            result.PageSize);
    }

    private static Expression<Func<Game, bool>> GetFilterCondition(GetGamesQuery request)
    {
        return game =>
            ((request.Platform == null)
                || (game.Platform.PublicId == request.Platform))
            && ((request.Categories == null)
                || game.GameCategoryMappings.Any(m => request.Categories.Contains(m.Category.PublicId)))
            && ((request.GameTypes == null)
                || game.GameTypeMappings.Any(m => request.GameTypes.Contains(m.Type.PublicId)))
            && ((request.Keyword == null)
                || game.PublicId.ToString() == request.Keyword.Trim()
                || game.Name.Contains(request.Keyword.Trim()));
    }
}
