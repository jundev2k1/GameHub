using game_x.application.Contract.Infrastructure.Caching.Rewards;
using game_x.application.Exceptions;
using game_x.application.Features.Rewards.Commands.MissionRewards.Create;
using game_x.application.Features.Rewards.Commands.Missions.Create;

namespace game_x.api.Controllers.BackOffice.Rewards;

[Authorize(Roles = AppRoles.Admin)]
[Route("/api/back-office/missions")]
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
        var result = await cache.GetDetail(id, ct);
        if (result == null)
            throw new NotFoundException(MessageCode.Reward.MissionNotFound);
        return ApiResponseFactory.Ok(result);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateAsync(CreateMissionCommand cmd, CancellationToken ct = default)
    {
        var result = await Mediator.Send(cmd, ct);
        return ApiResponseFactory.Created(result);
    }
    
    [HttpPost("{missionId:guid}/rewards")]
    public async Task<IActionResult> AddRewardAsync(Guid missionId, CreateMissionRewardCommand cmd, CancellationToken ct = default)
    {
        var result = await Mediator.Send(cmd with { MissionId = missionId }, ct);
        return ApiResponseFactory.Created(result);
    }
}