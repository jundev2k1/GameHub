using game_x.application.Contract.Infrastructure.ExternalApi.Atg;
using game_x.application.Features.Games.Client.Commands.Etl998.ModifyBettingLimit;
using game_x.application.Features.Games.Client.Commands.Etl998.SearchRecord;

namespace game_x.api.Controllers.Client.Game;

[Authorize(Roles = AppRoles.User)]
[Route("/api/user")]
public sealed class GameTestController(IAtgService atgService) : BaseApiController
{
    [HttpPost("game-etl998/games/search-record")]
    public async Task<IActionResult> SearchGameRecordsAsync(SearchRecordCommand command)
    {
        var result = await Mediator.Send(command);
        return ApiResponseFactory.Ok(result);
    }
    
    [HttpGet("game-etl998/games/modify-betting-limit")]
    public Task<IActionResult> GetBettingLimitAsync()
    {
        var result = BettingLimitGroups.All.Select(x => x.Value);
        return Task.FromResult(ApiResponseFactory.Ok(result));
    }
    
    [HttpPost("game-etl998/games/modify-betting-limit")]
    public async Task<IActionResult> ModifyBettingLimitAsync(ModifyBettingLimitCommand command)
    {
        var result = await Mediator.Send(command);
        return ApiResponseFactory.Ok(result);
    }
    
    [HttpGet("atg/game-providers")]
    public async Task<IActionResult> GameProvidersAsync()
    {
        var result = await atgService.GetGameProvidersAsync();
        return ApiResponseFactory.Ok(result);
    }
    
    [HttpGet("atg/games")]
    public async Task<IActionResult> GetGamesAsync()
    {
        var result = await atgService.GetGamesAsync();
        return ApiResponseFactory.Ok(result);
    }
}