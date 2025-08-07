using game_x.application.Features.Games.Commands.LoginGame;

namespace game_x.api.Controllers.Client.Game;

[Authorize(Roles = AppRoles.User)]
[Route("/api/user/game")]
public sealed class GameController : BaseApiController
{
    [HttpPost("auth/login")]
    public async Task<IActionResult> LoginAsync(LoginGameCommand command)
    {
        var result = await Mediator.Send(command);
        return ApiResponseFactory.Ok(result);
    }
}
