using game_x.application.Features.LiveStreams.Commands.JoinLiveStream;

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
}
