using game_x.application.Features.NavigationItems.Common.Queries.GetActiveNavigationItems;

namespace game_x.api.Controllers.Common;

[Route("/api/navigations")]
public sealed class NavigationController : BaseApiController
{
    [HttpGet("current/items")]
    public async Task<IActionResult> GetAllItemsAsync(CancellationToken ct = default)
    {
        var result = await Mediator.Send(new GetActiveNavigationItemsQuery(), ct);
        return ApiResponseFactory.Ok(result);
    }
}
