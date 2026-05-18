using game_x.application.Contract.Infrastructure.Caching.Rewards;
using game_x.application.Features.Rewards.Commands.CreateRewardDefinition;
using game_x.application.Features.Rewards.Commands.RemoveRewardDefinition;

namespace game_x.api.Controllers.BackOffice.Rewards;

[Authorize(Roles = AppRoles.Admin)]
[Route("/api/back-office/reward-definitions")]
public sealed class RewardController(IRewardDefinitionCacheService service) : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetListAsync(CancellationToken ct = default)
    {
        var result = await service.GetAll(ct);
        return ApiResponseFactory.Ok(result ?? []);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateAsync(CreateRewardDefinitionCommand cmd, CancellationToken ct = default)
    {
        var result = await Mediator.Send(cmd, ct);
        return ApiResponseFactory.Created(result);
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> RemoveAsync(Guid id, CancellationToken ct = default)
    {
        var result = await Mediator.Send(new RemoveRewardDefinitionCommand(id), ct);
        return ApiResponseFactory.Created(result);
    }
}