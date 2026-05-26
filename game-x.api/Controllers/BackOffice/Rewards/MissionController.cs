using game_x.application.Contract.Infrastructure.Caching.Rewards;
using game_x.application.Exceptions;
using game_x.application.Features.Rewards.Commands.Missions.Create;
using game_x.application.Features.Rewards.Commands.Missions.Remove;
using game_x.application.Features.Rewards.Commands.Missions.SyncItems;
using game_x.application.Features.Rewards.Commands.Missions.Update;

namespace game_x.api.Controllers.BackOffice.Rewards;

[Authorize(Roles = AppRoles.Admin)]
[Route("/api/back-office/missions")]
public sealed class MissionController(IMissionCacheService cache) : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetListAsync(CancellationToken ct = default)
    {
        var result = await cache.GetAllByAdmin(ct);
        return ApiResponseFactory.Ok(result);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetDetailAsync(Guid id, CancellationToken ct = default)
    {
        var result = await cache.GetDetailByAdmin(id, ct);
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
    
    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, UpdateMissionCommand cmd, CancellationToken ct = default)
    {
        var result = await Mediator.Send(cmd with {Id = id}, ct);
        return ApiResponseFactory.Created(result);
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> RemoveAsync(Guid id, CancellationToken ct = default)
    {
        var result = await Mediator.Send(new RemoveMissionCommand(id), ct);
        return ApiResponseFactory.Created(result);
    }
    
    [HttpPut("{missionId:guid}/rewards")]
    public async Task<IActionResult> UpdateRewardAsync(Guid missionId, SyncMissionRewardCommand cmd, CancellationToken ct = default)
    {
        var result = await Mediator.Send(cmd with { MissionId = missionId }, ct);
        return ApiResponseFactory.Created(result);
    }
}