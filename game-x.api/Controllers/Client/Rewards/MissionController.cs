using game_x.application.Contract.Infrastructure.Caching.Rewards;
using game_x.application.Features.Rewards.Commands.Missions.Claim;
using game_x.application.Features.Rewards.Queries.Missions.GetByUser;

namespace game_x.api.Controllers.Client.Rewards;

[Authorize(Roles = AppRoles.User)]
[Route("/api/user")]
public sealed class MissionController(IMissionCacheService cache) : BaseApiController
{
    [HttpGet("missions")]
    public async Task<IActionResult> GetListAsync(CancellationToken ct = default)
    {
        var result = await cache.GetAllByUser(ct);
        return ApiResponseFactory.Ok(result ?? []);
    }
    
    [HttpGet("missions/{id:guid}")]
    public async Task<IActionResult> GetDetailAsync(Guid id, CancellationToken ct = default)
    {
        var result = await Mediator.Send(new GetMissionDetailByUserQuery(id), ct);
        return ApiResponseFactory.Ok(result);
    }
    
    [HttpPost("mission-claims/{claimId:guid}")]
    public async Task<IActionResult> ClaimAsync(Guid claimId, CancellationToken ct = default)
    {
        var result = await Mediator.Send(new ClaimMissionRewardCommand(claimId), ct);
        return ApiResponseFactory.Ok(result);
    }
}