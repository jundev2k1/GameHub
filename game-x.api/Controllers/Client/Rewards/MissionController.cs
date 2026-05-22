using game_x.application.Contract.Infrastructure.Caching.Rewards;
using game_x.application.Features.Rewards.Commands.Missions.Claim;
using game_x.application.Features.Rewards.Queries.Missions.GetByUser;

namespace game_x.api.Controllers.Client.Rewards;

[Authorize(Roles = AppRoles.User)]
[Route("/api/user/missions")]
public sealed class MissionController(IMissionCacheService cache) : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetListAsync(CancellationToken ct = default)
    {
        var result = await cache.GetAll(ct);
        return ApiResponseFactory.Ok(result);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetDetailAsync(Guid id, CancellationToken ct = default)
    {
        var result = await Mediator.Send(new GetMissionDetailByUserQuery(id), ct);
        return ApiResponseFactory.Ok(result);
    }
    
    [HttpPost("{missionId:guid}/claims/{claimId:guid}")]
    public async Task<IActionResult> ClaimAsync(Guid missionId, Guid claimId, CancellationToken ct = default)
    {
        var result = await Mediator.Send(new ClaimMissionRewardCommand(missionId, claimId), ct);
        return ApiResponseFactory.Ok(result);
    }
}