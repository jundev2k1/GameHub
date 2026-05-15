using game_x.application.Features.Rewards.Commands.RewardPoolSpin;

namespace game_x.api.Controllers.Client.Rewards;

[Authorize(Roles = AppRoles.User)]
[Route("/api/user")]
public sealed class RewardController : BaseApiController
{
    [HttpPost("reward-pools/{id:guid}/spin")]
    public async Task<IActionResult> SpinAsync(Guid id, CancellationToken ct = default)
    {
        var result = await Mediator.Send(new RewardPoolSpinCommand(id), ct);
        return ApiResponseFactory.Created(result);
    }
}