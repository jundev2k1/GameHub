using game_x.application.Features.LiveStreams.Commands.CreateCategory;
using game_x.application.Features.LiveStreams.Commands.DeleteCategory;
using game_x.application.Features.LiveStreams.Commands.UpdateCategory;
using game_x.application.Features.LiveStreams.Commands.UpdateCategoryStatus;

namespace game_x.api.Controllers.BackOffice.LiveStream;

[Route("/back-office/livestream-categories")]
public sealed class LiveStreamCategoryController : BaseApiController
{
    [Authorize(Roles = AppRoles.Admin)]
    [HttpPost]
    public async Task<IActionResult> CreateLiveStreamCategoryAsync(CreateCategoryCommand command)
    {
        await Mediator.Send(command);
        return ApiResponseFactory.NoContent();
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateLiveStreamCategoryAsync(Guid id, UpdateCategoryCommand command)
    {
        await Mediator.Send(command with { Id = id });
        return ApiResponseFactory.NoContent();
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateLiveStreamCategoryStatusAsync(Guid id, UpdateCategoryStatusCommand command)
    {
        await Mediator.Send(command with { Id = id });
        return ApiResponseFactory.NoContent();
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteLiveStreamCategoryAsync(Guid id, DeleteCategoryCommand command)
    {
        await Mediator.Send(command with { Id = id });
        return ApiResponseFactory.NoContent();
    }
}
