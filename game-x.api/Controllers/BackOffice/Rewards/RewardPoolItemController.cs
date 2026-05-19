using game_x.application.Features.Rewards.Commands.RewardPoolItems.Remove;
using game_x.application.Features.Rewards.Commands.RewardPoolItems.Update;

namespace game_x.api.Controllers.BackOffice.Rewards;

[Authorize(Roles = AppRoles.Admin)]
[Route("/api/back-office/reward-pool-items")]
public sealed class RewardPoolItemController : BaseApiController
{
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> RemoveItemAsync(Guid id, CancellationToken ct = default)
    {
        var result = await Mediator.Send(new RemoveRewardPoolItemCommand(id), ct);
        return ApiResponseFactory.Created(result);
    }
    
    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, UpdateRewardPoolItemCommand cmd, CancellationToken ct = default)
    {
        var result = await Mediator.Send(cmd with {Id = id}, ct);
        return ApiResponseFactory.Created(result);
    }
}