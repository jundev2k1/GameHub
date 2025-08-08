using game_x.application.Features.Games.Commands.LoginGame;
using game_x.application.Features.Games.Queries.WalletGame;

namespace game_x.api.Controllers.Client.Game;

[Authorize(Roles = AppRoles.User)]
[Route("/api/user/game")]
public sealed class GameController : BaseApiController
{
    [HttpPost("auth/login")]
    public async Task<IActionResult> LoginAsync(LoginGameCommand request)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress.ToStringOrEmpty();
        var command = request with { IpAddress = ipAddress };
        var result = await Mediator.Send(command);
        return ApiResponseFactory.Ok(result);
    }

    [HttpGet("me/wallet")]
    public async Task<IActionResult> GetWalletAsync()
    {
        var query = new GetWalletGameQuery();
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }
}
