using game_x.application.Features.Rewards.Commands.AddUserCatalogItem;

namespace game_x.api.Controllers.BackOffice.Rewards;

[Authorize(Roles = AppRoles.Admin)]
[Route("/api/back-office/user-inventories")]
public sealed class UserInventoryController : BaseApiController
{
    [HttpPost("{userId}")]
    public async Task<IActionResult> CreateAsync(string userId, AddUserCatalogItemCommand cmd, CancellationToken ct = default)
    {
        var result = await Mediator.Send(cmd with { UserId = userId }, ct);
        return ApiResponseFactory.Created(result);
    }
}