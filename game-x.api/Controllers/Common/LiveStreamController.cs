using game_x.application.Features.LiveStreams.Queries.GetActiveStreams;

namespace game_x.api.Controllers.Common;

[Route("/api/livestreams")]
public sealed class LiveStreamController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetActiveStreamsAsync([FromQuery] GetActiveStreamsQuery query)
    {
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }
}
