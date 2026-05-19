using game_x.application.Common.Files;
using game_x.application.Features.NavigationItems.Admin.Commands.CreateNavigationItem;
using game_x.application.Features.NavigationItems.Admin.Commands.DeleteNavigationItem;
using game_x.application.Features.NavigationItems.Admin.Commands.UpdateNavigationItem;
using game_x.application.Features.NavigationItems.Admin.Commands.UpdateNavigationItemIcon;
using game_x.application.Features.NavigationItems.Admin.Commands.UpdateNavigationItemStatus;
using game_x.application.Features.NavigationItems.Admin.Commands.UpdateNavigationItemTranslations;
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
    [HttpPost]
    public async Task<IActionResult> CreateItemAsync(CreateNavigationItemCommand command, CancellationToken ct = default)
    {
        await Mediator.Send(command, ct);
        return ApiResponseFactory.NoContent();
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPost("{id:guid}/icon")]
    public async Task<IActionResult> UploadNavigationIconAsync(
        Guid id,
        IFormFile icon,
        CancellationToken ct = default)
    {
        var command = new UpdateNavigationItemIconCommand(id, FileUpload.FromFormFile(icon));
        var url = await Mediator.Send(command, ct);
        return ApiResponseFactory.Ok(url);
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateNavigationAsync(
        Guid id,
        UpdateNavigationItemCommand command,
        CancellationToken ct = default)
    {
        var result = await Mediator.Send(command with { Id = id }, ct);
        return ApiResponseFactory.Ok(result);
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPost("{id:guid}/translations")]
    public async Task<IActionResult> UpsertGameTranslationsAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateNavigationItemTranslationsCommand command)
    {
        await Mediator.Send(command with { Id = id });
        return ApiResponseFactory.NoContent(code: MessageCode.System.Updated);
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatusNavigationAsync(
        Guid id,
        UpdateNavigationItemStatusCommand command,
        CancellationToken ct = default)
    {
        var result = await Mediator.Send(command with { Id = id }, ct);
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
