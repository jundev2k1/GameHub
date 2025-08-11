using game_x.api.Common;
using game_x.application.Common.Filters;
using game_x.application.Features.ChainTransactions.Admin.Commands.AdminReviewWithdrawalOrder;
using game_x.application.Features.ChainTransactions.Admin.Queries.GetTransactionCriteriaByAdmin;
using game_x.application.Features.ChainTransactions.Admin.Queries.GetTransactionDetailById;
using game_x.application.Features.ChainTransactions.Client.Queries.GetUsdtLedgerDetail;

namespace game_x.api.Controllers.Admin.Chain;

[Authorize(Roles = AppRoles.Admin)]
[Route("/api/admin")]
public sealed class ChainController : BaseApiController
{
    [HttpPost("withdrawal/{orderId}/review")]
    public async Task<IActionResult> ReviewWithdrawalOrderAsync(Guid orderId, [FromBody] AdminReviewWithdrawalOrderCommand command, CancellationToken ct)
    {
        var result = await Mediator.Send(command with {OrderId = orderId}, ct);
        return ApiResponseFactory.Ok(result);
    }

    [HttpGet("chain-transactions")]
    public async Task<IActionResult> GetTransactionByCriteria([AsParameters] SearchCriteriaRequest parameters)
    {
        var filters = QueryConverter.ToFilters(parameters.Filters, parameters.Keyword);
        var sorts = QueryConverter.ToSorts(parameters.Sorts);
        var query = new GetTransactionCriteriaByAdminQuery(
            filters,
            sorts,
            parameters.PageNumber,
            parameters.PageSize);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }

    [HttpGet("chain-transactions/{transactionId}")]
    public async Task<IActionResult> GetTransactionByIdAsync(Guid transactionId)
    {
        var query = new GetTransactionDetailByIdQuery(transactionId);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }
}
