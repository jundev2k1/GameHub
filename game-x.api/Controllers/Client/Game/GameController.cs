using game_x.api.Common;
using game_x.application.Common;
using game_x.application.Common.Filters;
using game_x.application.Features.Games.Commands.GameWallet.Deposit;
using game_x.application.Features.Games.Commands.GameWallet.Withdrawal;
using game_x.application.Features.Games.Commands.LoginGame;
using game_x.application.Features.Games.Queries.GetMyGameTransactionDetail;
using game_x.application.Features.Games.Queries.GetMyGameTransactions;
using game_x.application.Features.Games.Queries.WalletGame;

namespace game_x.api.Controllers.Client.Game;

[Authorize(Roles = AppRoles.User)]
[Route("/api/user/game")]
public sealed class GameController : BaseApiController
{
    [HttpPost("auth/login")]
    public async Task<IActionResult> LoginAsync(LoginGameCommand request)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress.ToStringOrEmpty();
        var command = request with { IpAddress = ipAddress };
        var result = await Mediator.Send(command);
        return ApiResponseFactory.Ok(result);
    }

    [HttpGet("me/wallet-G598")]
    public async Task<IActionResult> GetWalletAsync()
    {
        var query = new GetWalletGameQuery();
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }

    [HttpPost("me/wallet-G598/deposit")]
    public async Task<IActionResult> DepositAsync(WalletDepositCommand command)
    {
        var result = await Mediator.Send(command);
        return ApiResponseFactory.Ok(result);
    }

    [HttpPost("me/wallet-G598/withdrawal")]
    public async Task<IActionResult> WithdrawalAsync(WalletWithdrawalCommand command)
    {
        var result = await Mediator.Send(command);
        return ApiResponseFactory.Ok(result);
    }

    [HttpGet("game-codes")]
    public IActionResult GetGameCode()
    {
        return ApiResponseFactory.Ok(GameCodeProvider.All());
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
