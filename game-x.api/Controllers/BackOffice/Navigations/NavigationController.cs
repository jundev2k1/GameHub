using game_x.application.Features.NavigationItems.Admin.Commands.DeleteNavigationItem;
using game_x.application.Features.NavigationItems.Admin.Queries.GetAllNavigationItems;

namespace game_x.api.Controllers.BackOffice.Navigations;

[Route("/api/back-office/navigations")]
public sealed class NavigationController : BaseApiController
{
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Cs}")]
    [HttpGet]
    public async Task<IActionResult> GetAllNavigationItemListAsync(CancellationToken ct = default)
    {
        var result = await Mediator.Send(new GetAllNavigationItemsQuery(), ct);
        return ApiResponseFactory.Ok(result);
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteItemAsync(Guid id, CancellationToken ct = default)
    {
        var command = new DeleteNavigationItemCommand(id);
        await Mediator.Send(command, ct);
        return ApiResponseFactory.NoContent();
    }
}
