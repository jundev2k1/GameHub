using game_x.api.Common;
using game_x.application.Common.Filters;
using game_x.application.Features.Games.Client.Commands.GameWallet.Deposit;
using game_x.application.Features.Games.Client.Commands.GameWallet.Withdrawal;
using game_x.application.Features.Games.Client.Commands.LoginGame;
using game_x.application.Features.Games.Client.Queries.GetGameReport;
using game_x.application.Features.Games.Client.Queries.GetMyGameTransactionDetail;
using game_x.application.Features.Games.Client.Queries.GetMyGameTransactions;

namespace game_x.api.Controllers.Client.Game;

[Authorize(Roles = AppRoles.User)]
[Route("/api/user/game")]
public sealed class GameController : BaseApiController
{
    [HttpPost("{gamePlatformId:guid}/login")]
    public async Task<IActionResult> LoginAsync(Guid gamePlatformId, LoginGameCommand request)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress.ToStringOrEmpty();
        var command = request with { GamePlatformId = gamePlatformId, IpAddress = ipAddress };
        var result = await Mediator.Send(command);
        return ApiResponseFactory.Ok(result);
    }

    [HttpPost("platforms/{platformId:guid}/deposit")]
    public async Task<IActionResult> DepositAsync(Guid platformId, WalletDepositCommand command)
    {
        var result = await Mediator.Send(command with { PlatformId = platformId });
        return ApiResponseFactory.Ok(result);
    }

    [HttpPost("platforms/{platformId:guid}/withdrawal")]
    public async Task<IActionResult> WithdrawalAsync(Guid platformId, WalletWithdrawalCommand command)
    {
        var result = await Mediator.Send(command with { PlatformId = platformId });
        return ApiResponseFactory.Ok(result);
    }

    [HttpPost("platforms/{platformId:guid}/report")]
    public async Task<IActionResult> ReportAsync(Guid platformId, GetGameReportQuery query)
    {
        var result = await Mediator.Send(query with { PlatformId = platformId });
        return ApiResponseFactory.Ok(result);
    }

    [HttpGet("transactions/me")]
    public async Task<IActionResult> GetTransactionByCriteriaAsync([AsParameters] SearchCriteriaRequest parameters)
    {
        var filters = QueryConverter.ToFilters(parameters.Filters, parameters.Keyword);
        var sorts = QueryConverter.ToSorts(parameters.Sorts);
        var query = new GetMyGameTransactionsQuery(
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
        var query = new GetMyGameTransactionDetailQuery(transactionId);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }
}