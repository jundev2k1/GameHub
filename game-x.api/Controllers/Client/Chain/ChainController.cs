using game_x.application.Common.Filters;
using game_x.application.Features.Transactions.Client.Commands.TraceV1.TronUsdtWithdrawal;
using game_x.application.Features.Transactions.Client.Commands.TraceV1.TronUsdtDeposit;
using game_x.application.Features.Transactions.Client.Queries.GetMyTransactionDetail;
using game_x.application.Features.Transactions.Client.Queries.GetMyTransactions;
using game_x.api.Dtos;
using game_x.application.Features.Transactions.Client.Commands.TraceV1.TransferBetweenFriends;

namespace game_x.api.Controllers.Client.Chain;

/// <summary>Uxm Integration</summary>
[Authorize(Roles = AppRoles.User)]
[Route("/api/user/chain-transactions")]
public sealed class ChainController : BaseApiController
{
    [HttpPost("withdrawal")]
    public async Task<IActionResult> CreateWithdrawalTransactionAsync(TronUsdtWithdrawalCommand command, CancellationToken ct = default)
    {
        var result = await Mediator.Send(command, ct);
        return ApiResponseFactory.Ok(result);
    }

    [HttpPost("deposit")]
    public async Task<IActionResult> CreateDepositTransactionAsync(TronUsdtDepositCommand command, CancellationToken ct = default)
    {
        var result = await Mediator.Send(command, ct);
        return ApiResponseFactory.Ok(result);
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetTransactionByCriteriaAsync([AsParameters] GetTransactionsRequest parameters, CancellationToken ct = default)
    {
        var paramExtends = new Dictionary<string, string>();
        if (parameters.TransactionStatuses.IsNotNullOrEmpty())
            paramExtends.Add("statuses", parameters.TransactionStatuses);

        var filters = QueryConverter.ToFilters(parameters.Filters, parameters.Keyword, paramExtends);
        var sorts = QueryConverter.ToSorts(parameters.Sorts);
        var query = new GetMyTransactionsQuery(
            filters,
            sorts,
            parameters.PageNumber,
            parameters.PageSize);
        var result = await Mediator.Send(query, ct);
        return ApiResponseFactory.Ok(result);
    }

    [HttpGet("{transactionId:guid}")]
    public async Task<IActionResult> GetTransactionByIdAsync(Guid transactionId, CancellationToken ct = default)
    {
        var query = new GetMyTransactionDetailQuery(transactionId);
        var result = await Mediator.Send(query, ct);
        return ApiResponseFactory.Ok(result);
    }
    
    [HttpPost("transfer")]
    public async Task<IActionResult> CreateTransferAsync(TransferBetweenFriendsCommand command, CancellationToken ct = default)
    {
        var result = await Mediator.Send(command, ct);
        return ApiResponseFactory.Ok(result);
    }
}