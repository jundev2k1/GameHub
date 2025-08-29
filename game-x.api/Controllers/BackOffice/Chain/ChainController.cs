using game_x.api.Common;
using game_x.application.Common.Filters;
using game_x.application.Features.ChainTransactions.Admin.Commands.AdminReviewWithdrawalOrder;
using game_x.application.Features.ChainTransactions.Admin.Queries.GetTransactionCriteriaByAdmin;
using game_x.application.Features.ChainTransactions.Admin.Queries.GetTransactionDetailById;

namespace game_x.api.Controllers.BackOffice.Chain;

[Route("/api/back-office/chains")]
public sealed class ChainController : BaseApiController
{
    [Authorize(Roles = AppRoles.Admin)]
    [HttpPost("withdrawal/{orderId:guid}/review")]
    public async Task<IActionResult> ReviewWithdrawalOrderAsync(Guid orderId, [FromBody] AdminReviewWithdrawalOrderCommand command, CancellationToken ct)
    {
        var result = await Mediator.Send(command with { OrderId = orderId }, ct);
        return ApiResponseFactory.Ok(result);
    }

    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Cs}")]
    [HttpGet("transactions")]
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

    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Cs}")]
    [HttpGet("transactions/{transactionId:guid}")]
    public async Task<IActionResult> GetTransactionByIdAsync(Guid transactionId)
    {
        var query = new GetTransactionDetailByIdQuery(transactionId);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }
}
