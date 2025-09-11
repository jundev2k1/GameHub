using game_x.api.Common;
using game_x.application.Common.Filters;
using game_x.application.Features.LiveStreams.Queries.GetActiveStreams;

namespace game_x.api.Controllers.Common;

[Route("/api/livestreams")]
public sealed class LiveStreamController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetActiveStreamsAsync([AsParameters] SearchCriteriaRequest parameters)
    {
        var query = new GetActiveStreamsQuery(
            QueryConverter.ToFilters(parameters.Filters),
            QueryConverter.ToSorts(parameters.Sorts),
            parameters.PageNumber ?? 1,
            parameters.PageSize ?? 20);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }
}
