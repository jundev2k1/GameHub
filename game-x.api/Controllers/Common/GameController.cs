using game_x.api.Dtos;
using game_x.application.Features.Games.Client.Queries.GetGames;

namespace game_x.api.Controllers.Common;

[Route("/api/game")]
public sealed class GameController : BaseApiController
{
    [HttpGet("list")]
    public async Task<IActionResult> GetGameListAsync([AsParameters] GetGamesRequest request)
    {
        var query = new GetGamesQuery(
            request.Keyword,
            request.Platform,
            request.Category,
            request.GameType,
            request.PageNumber ?? 1,
            request.PageSize ?? 20);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }
}
