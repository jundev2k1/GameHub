using game_x.application.Contract.Infrastructure.Caching.Rewards;
using game_x.application.Features.Rewards.Commands.CreateRewardPoolItem;

namespace game_x.api.Controllers.BackOffice.Rewards;

[Authorize(Roles = AppRoles.Admin)]
[Route("/api/back-office/reward-pools")]
public sealed class RewardPoolController(
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
    
    [HttpPost("{id:guid}/items")]
    public async Task<IActionResult> AddRewardAsync(Guid id, CreateRewardPoolItemCommand cmd, CancellationToken ct = default)
    {
        var result = await Mediator.Send(cmd with { RewardPoolId = id }, ct);
        return ApiResponseFactory.Created(result);
    }
}