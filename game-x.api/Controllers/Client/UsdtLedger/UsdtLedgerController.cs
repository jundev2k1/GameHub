using game_x.api.Common;
using game_x.application.Common.Filters;
using game_x.application.Features.ChainTransactions.Client.Queries.GetUsdtLedgerCriteriaByUser;
using game_x.application.Features.ChainTransactions.Client.Queries.GetUsdtLedgerDetail;

namespace game_x.api.Controllers.Client.UsdtLedger;

[Authorize(Roles = AppRoles.User)]
[Route("/api/usdt-ledgers")]
public class UsdtLedgerController : BaseApiController
{
    /// <summary>
    ///     Retrieve the personal transaction history list
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetMyUsdtLedgerByCriteria([AsParameters] SearchCriteriaRequest parameters)
    {
        var filters = QueryConverter.ToFilters(parameters.Filters, parameters.Keyword);
        var sorts = QueryConverter.ToSorts(parameters.Sorts);
        var query = new GetUserUsdtLedgerCriteriaByUserQuery(
            filters,
            sorts,
            parameters.PageNumber,
            parameters.PageSize);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }

    [HttpGet("{ledgerId}")]
    public async Task<IActionResult> GetUsdtLedgerById(Guid ledgerId)
    {
        var query = new GetUserUsdtLedgerDetailQuery(ledgerId);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }
}