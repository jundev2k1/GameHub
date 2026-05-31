using game_x.application.Features.GetUnderReviewStatistics.Admin.Queries.GetTransactionStatistics;

namespace game_x.api.Controllers.BackOffice.Statistics;

[Route("api/back-office/statistics")]
public sealed class StatisticController : BaseApiController
{
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Cs}")]
    [HttpGet("under-review")]
    public async Task<IActionResult> GetUnderReviewStatisticAsync()
    {
        var query = new GetUnderReviewStatisticsQuery();
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }
}