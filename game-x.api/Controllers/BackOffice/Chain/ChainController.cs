using game_x.api.Dtos;
using game_x.application.Common.Filters;
using game_x.application.Features.Transactions.Admin.Commands.AdminReviewWithdrawalOrder;
using game_x.application.Features.Transactions.Admin.Commands.CancelTransaction;
using game_x.application.Features.Transactions.Admin.Commands.CreateTransaction;
using game_x.application.Features.Transactions.Admin.Queries.GetTransactionCriteriaByAdmin;
using game_x.application.Features.Transactions.Admin.Queries.GetTransactionDetailById;

namespace game_x.api.Controllers.BackOffice.Chain;

[Authorize(Roles = AppRoles.Admin)]
[Route("/api/back-office/chain-transactions")]
public sealed class ChainController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetTransactionByCriteria([AsParameters] GetTransactionsRequest parameters)
    {
        var paramExtends = new Dictionary<string, string>();
        if (parameters.TransactionStatuses.IsNotNullOrEmpty())
            paramExtends.Add("statuses", parameters.TransactionStatuses);

        var filters = QueryConverter.ToFilters(parameters.Filters, parameters.Keyword, paramExtends);
        var sorts = QueryConverter.ToSorts(parameters.Sorts);
        var query = new GetTransactionCriteriaByAdminQuery(
            filters,
            sorts,
            parameters.PageNumber,
            parameters.PageSize);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }

    [HttpGet("{transactionId:guid}")]
    public async Task<IActionResult> GetTransactionByIdAsync(Guid transactionId)
    {
        var query = new GetTransactionDetailByIdQuery(transactionId);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTransactionAsync(CreateTransactionCommand command)
    {
        await Mediator.Send(command);
        return ApiResponseFactory.NoContent();
    }

    [HttpPost("{transactionId:guid}/cancellation")]
    public async Task<IActionResult> CancelTransactionAsync(Guid transactionId)
    {
        var command = new CancelTransactionCommand(transactionId);
        await Mediator.Send(command);
        return ApiResponseFactory.NoContent();
    }

    [HttpPost("{orderId:guid}/withdrawal-review")]
    public async Task<IActionResult> ReviewWithdrawalOrderAsync(Guid orderId, [FromBody] AdminReviewWithdrawalOrderCommand command, CancellationToken ct)
    {
        var result = await Mediator.Send(command with { OrderId = orderId }, ct);
        return ApiResponseFactory.Ok(result);
    }
}
