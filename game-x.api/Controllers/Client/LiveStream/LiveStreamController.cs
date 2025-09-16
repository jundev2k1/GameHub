using game_x.application.Features.LiveStreams.Commands.JoinLiveStream;
using game_x.application.Features.LiveStreams.Commands.PerformAction;

namespace game_x.api.Controllers.Client.LiveStream;

[Route("/api/user/livestreams")]
public sealed class LiveStreamController : BaseApiController
{
    [Authorize(Roles = AppRoles.User)]
    [HttpPost("{id:guid}")]
    public async Task<IActionResult> JoinLiveStreamAsync(Guid id)
    {
        var query = new JoinLiveStreamCommand(id);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }

    [Authorize(Roles = AppRoles.User)]
    [HttpPost("{streamId}/viewers/{viewerId}/actions")]
    public async Task<IActionResult> KickViewerAsync(Guid streamId, string viewerId, PerformActionCommand command)
    {
        await Mediator.Send(command with { StreamId = streamId, ViewerId = viewerId });
        return ApiResponseFactory.NoContent();
    }
}
