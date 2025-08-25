using game_x.api.Common;
using game_x.application.Common.Filters;
using game_x.application.Features.Games.Client.Queries.GetGameTransactionDetail;
using game_x.application.Features.Games.Client.Queries.GetGameTransactions;

namespace game_x.api.Controllers.Admin.Game;

[Authorize(Roles = AppRoles.Admin)]
[Route("/api/admin/game")]
public sealed class GameController : BaseApiController
{
    [HttpGet("transactions")]
    public async Task<IActionResult> GetTransactionByCriteriaAsync([AsParameters] SearchCriteriaRequest parameters)
    {
        var filters = QueryConverter.ToFilters(parameters.Filters, parameters.Keyword);
        var sorts = QueryConverter.ToSorts(parameters.Sorts);
        var query = new GetGameTransactionsQuery(
            filters,
            sorts,
            parameters.PageNumber,
            parameters.PageSize);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }

    [HttpGet("transactions/{transactionId}")]
    public async Task<IActionResult> GetTransactionByIdAsync(Guid transactionId)
    {
        var query = new GetGameTransactionDetailQuery(transactionId);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }
}
