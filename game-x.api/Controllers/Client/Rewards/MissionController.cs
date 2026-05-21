using game_x.application.Contract.Infrastructure.Caching.Rewards;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo.Reward;
using game_x.application.Exceptions;
using game_x.application.Features.Rewards.Commands.Missions.Claim;

namespace game_x.api.Controllers.Client.Rewards;

[Authorize(Roles = AppRoles.User)]
[Route("/api/user/missions")]
public sealed class MissionController(
    IUserAccessor userAccessor,
    IMissionCacheService cache,
    IMissionRepo missionRepo) : BaseApiController
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
        string userId = userAccessor.GetUserId();
        var result = await missionRepo.GetDetailByUserAsync(userId, id, ct);
        if (result == null)
            throw new NotFoundException(MessageCode.Reward.MissionNotFound);
        return ApiResponseFactory.Ok(result);
    }
    
    [HttpPost("{missionId:guid}/claims/{claimId:guid}")]
    public async Task<IActionResult> ClaimAsync(Guid missionId, Guid claimId, CancellationToken ct = default)
    {
        var result = await Mediator.Send(new ClaimMissionRewardCommand(missionId, claimId), ct);
        return ApiResponseFactory.Ok(result);
    }
}