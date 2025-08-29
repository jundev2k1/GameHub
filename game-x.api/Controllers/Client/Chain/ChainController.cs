using game_x.api.Common;
using game_x.application.Common.Filters;
using game_x.application.Features.ChainTransactions.Client.Commands.TronUsdtWithdrawal;
using game_x.application.Features.ChainTransactions.Client.Commands.TronUsdtDeposit;
using game_x.application.Features.ChainTransactions.Client.Queries.GetMyTransactionDetail;
using game_x.application.Features.ChainTransactions.Client.Queries.GetMyTransactions;

namespace game_x.api.Controllers.Client.Chain;

[Authorize(Roles = AppRoles.User)]
[Route("/api/user/chain")]
public sealed class ChainController : BaseApiController
{
    [HttpPost("withdrawal")]
    public async Task<IActionResult> CreateWithdrawalTransactionAsync(TronUsdtWithdrawalCommand command, CancellationToken ct)
    {
        var result = await Mediator.Send(command, ct);
        return ApiResponseFactory.Ok(result);
    }

    [HttpPost("deposit")]
    public async Task<IActionResult> CreateDepositTransactionAsync(TronUsdtDepositCommand command, CancellationToken ct)
    {
        var result = await Mediator.Send(command, ct);
        return ApiResponseFactory.Ok(result);
    }
    
    [HttpGet("transactions/me")]
    public async Task<IActionResult> GetTransactionByCriteriaAsync([AsParameters] SearchCriteriaRequest parameters)
    {
        var filters = QueryConverter.ToFilters(parameters.Filters, parameters.Keyword);
        var sorts = QueryConverter.ToSorts(parameters.Sorts);
        var query = new GetMyTransactionsQuery(
            filters,
            sorts,
            parameters.PageNumber,
            parameters.PageSize);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }
    
    [HttpGet("transactions/{transactionId:guid}")]
    public async Task<IActionResult> GetTransactionByIdAsync(Guid transactionId)
    {
        var query = new GetMyTransactionDetailQuery(transactionId);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }
}
