using game_x.application.Features.Rewards.Commands.Missions.Claim;
using game_x.application.Features.Rewards.Commands.Missions.GenerateShareLink;
using game_x.application.Features.Rewards.Queries.Missions.GetDetailForUser;
using game_x.application.Features.Rewards.Queries.Missions.GetListForUser;
using game_x.domain.Enum.Rewards;

namespace game_x.api.Controllers.Client.Rewards;

[Authorize(Roles = AppRoles.User)]
[Route("/api/user")]
public sealed class MissionController : BaseApiController
{
    [HttpGet("missions")]
    public async Task<IActionResult> GetListAsync(
        [FromQuery(Name = "type")] MissionType? type, 
        CancellationToken ct = default)
    {
        var result = await Mediator.Send(new GetMissionListForUserQuery(type), ct);
        return ApiResponseFactory.Ok(result);
    }
    
    [HttpGet("missions/{id:guid}")]
    public async Task<IActionResult> GetDetailAsync(Guid id, CancellationToken ct = default)
    {
        var result = await Mediator.Send(new GetMissionDetailForUserQuery(id), ct);
        return ApiResponseFactory.Ok(result);
    }
    
    [HttpPost("missions/{id:guid}/share-link")]
    public async Task<IActionResult> GenerateShareLinkAsync(Guid id, CancellationToken ct = default)
    {
        var result = await Mediator.Send(new ShareLinkCommand(id), ct);
        return ApiResponseFactory.Ok(result);
    }
    
    [HttpPost("mission-claims/{claimId:guid}")]
    public async Task<IActionResult> ClaimAsync(Guid claimId, CancellationToken ct = default)
    {
        var result = await Mediator.Send(new ClaimMissionRewardCommand(claimId), ct);
        return ApiResponseFactory.Ok(result);
    }
}