using game_x.application.Contract.Infrastructure.ExternalApi.Atg;

namespace game_x.api.Controllers.Client.Game;

[Authorize(Roles = AppRoles.User)]
[Route("/api/user/atg")]
public sealed class AtgController(IAtgService service) : BaseApiController
{
    [HttpGet("game-providers")]
    public async Task<IActionResult> GameProvidersAsync()
    {
        var result = await service.GetGameProvidersAsync();
        return ApiResponseFactory.Ok(result);
    }
    
    [HttpGet("games")]
    public async Task<IActionResult> GetGamesAsync()
    {
        var result = await service.GetGamesAsync();
        return ApiResponseFactory.Ok(result);
    }
}