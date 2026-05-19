using game_x.application.Contract.Infrastructure.Caching.Rewards;
using game_x.application.Features.Rewards.Commands.RewardPools.Execute;

namespace game_x.api.Controllers.Client.Rewards;

[Authorize(Roles = AppRoles.User)]
[Route("/api/user/reward-pools")]
public sealed class RewardController(
    IRewardPoolCacheService poolService,
    IRewardPoolItemCacheService itemService) : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetListAsync(CancellationToken ct = default)
    {
        var result = await poolService.GetAll(ct);
        return ApiResponseFactory.Ok(result ?? []);
    }
    
    [HttpGet("{id:guid}/items")]
    public async Task<IActionResult> GetRewardListAsync(Guid id, CancellationToken ct = default)
    {
        var result = await itemService.GetAll(id, ct);
        return ApiResponseFactory.Ok(result ?? []);
    }
    
    [HttpPost("{id:guid}/execute")]
    public async Task<IActionResult> ExecuteAsync(Guid id, CancellationToken ct = default)
    {
        var result = await Mediator.Send(new RewardPoolExecuteCommand(id), ct);
        return ApiResponseFactory.Created(result);
    }
}