using game_x.application.Contract.Infrastructure.Caching.Rewards;
using game_x.application.Features.Rewards.Commands.CreateMission;

namespace game_x.api.Controllers.BackOffice.Rewards;

[Authorize(Roles = AppRoles.Admin)]
[Route("/api/back-office/missions")]
public sealed class MissionController(IMissionCacheService mission) : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetListAsync(CancellationToken ct = default)
    {
        var result = await mission.GetAll(ct);
        return ApiResponseFactory.Ok(result);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetDetailAsync(Guid id, CancellationToken ct = default)
    {
        var result = await mission.GetDetail(id, ct);
        return ApiResponseFactory.Ok(result);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateMissionCommand cmd, CancellationToken ct = default)
    {
        var result = await Mediator.Send(cmd, ct);
        return ApiResponseFactory.Created(result);
    }
}