using game_x.application.Contract.Infrastructure.Caching.Rewards;
using game_x.application.Features.Rewards.Commands.RewardPoolItems.BulkDelete;
using game_x.application.Features.Rewards.Commands.RewardPoolItems.BulkUpdate;
using game_x.application.Features.Rewards.Commands.RewardPoolItems.Create;
using game_x.application.Features.Rewards.Commands.RewardPools.Update;

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
    
    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, UpdateRewardPoolCommand cmd, CancellationToken ct = default)
    {
        var result = await Mediator.Send(cmd with {Id = id}, ct);
        return ApiResponseFactory.Created(result);
    }
    
    [HttpPatch("{id:guid}/items")]
    public async Task<IActionResult> UpdateItemsAsync(Guid id, BulkUpdateRewardPoolItemCommand cmd, CancellationToken ct = default)
    {
        var result = await Mediator.Send(cmd with {RewardPoolId = id}, ct);
        return ApiResponseFactory.Created(result);
    }
    
    [HttpDelete("{id:guid}/items")]
    public async Task<IActionResult> BulkDeleteItemsAsync(Guid id, BulkDeleteRewardPoolItemCommand cmd,
        CancellationToken ct = default)
    {
        await Mediator.Send(cmd with { RewardPoolId = id }, ct);
        return ApiResponseFactory.NoContent();
    }
}