using game_x.application.Features.Games.Client.Commands.Etl998.AccountBalance;
using game_x.application.Features.Games.Client.Commands.Etl998.CancelTransfer;
using game_x.application.Features.Games.Client.Commands.Etl998.ChangePassword;
using game_x.application.Features.Games.Client.Commands.Etl998.CheckAccountExistence;
using game_x.application.Features.Games.Client.Commands.Etl998.ConfirmTransfer;
using game_x.application.Features.Games.Client.Commands.Etl998.CreateAccount;
using game_x.application.Features.Games.Client.Commands.Etl998.ForwardGame;
using game_x.application.Features.Games.Client.Commands.Etl998.ModifyBettingLimit;
using game_x.application.Features.Games.Client.Commands.Etl998.PrepareTransfer;
using game_x.application.Features.Games.Client.Commands.Etl998.SearchRecord;
using game_x.application.Features.Games.Client.Commands.Etl998.SearchTransfer;

namespace game_x.api.Controllers.Client.Game;

[Authorize(Roles = AppRoles.User)]
[Route("/api/user/game-etl998")]
public sealed class Etl998Controller : BaseApiController
{
    [HttpPost("accounts")]
    public async Task<IActionResult> CreateNewAccountAsync(CreateAccountCommand command)
    {
        var result = await Mediator.Send(command);
        return ApiResponseFactory.Ok(result);
    }
    
    [HttpPost("accounts/existence")]
    public async Task<IActionResult> CheckAccountExistenceAsync(CheckAccountExistenceCommand command)
    {
        var result = await Mediator.Send(command);
        return ApiResponseFactory.Ok(result);
    }
    
    [HttpPost("accounts/balance")]
    public async Task<IActionResult> GetAccountBalanceAsync(AccountBalanceCommand command)
    {
        var result = await Mediator.Send(command);
        return ApiResponseFactory.Ok(result);
    }
    
    [HttpPost("accounts/change-password")]
    public async Task<IActionResult> ChangePasswordAsync(ChangePasswordCommand command)
    {
        var result = await Mediator.Send(command);
        return ApiResponseFactory.Ok(result);
    }
    
    [HttpPost("transfers/prepare")]
    public async Task<IActionResult> GetPrepareTransferAsync(PrepareTransferCommand command)
    {
        var result = await Mediator.Send(command);
        return ApiResponseFactory.Ok(result);
    }
    
    [HttpPost("transfers/confirm")]
    public async Task<IActionResult> ConfirmTransferAsync(ConfirmTransferCommand command)
    {
        var result = await Mediator.Send(command);
        return ApiResponseFactory.Ok(result);
    }
    
    [HttpPost("transfers/search")]
    public async Task<IActionResult> SearchTransferAsync(SearchTransferCommand command)
    {
        var result = await Mediator.Send(command);
        return ApiResponseFactory.Ok(result);
    }
    
    [HttpPost("transfers/cancel")]
    public async Task<IActionResult> CancelTransferAsync(CancelTransferCommand command)
    {
        var result = await Mediator.Send(command);
        return ApiResponseFactory.Ok(result);
    }
    
    [HttpPost("games/forward")]
    public async Task<IActionResult> ForwardGameAsync(ForwardGameCommand command)
    {
        var result = await Mediator.Send(command);
        return ApiResponseFactory.Ok(result);
    }
    
    [HttpPost("games/search-record")]
    public async Task<IActionResult> SearchGameRecordsAsync(SearchRecordCommand command)
    {
        var result = await Mediator.Send(command);
        return ApiResponseFactory.Ok(result);
    }
    
    [HttpPost("games/modify-betting-limit")]
    public async Task<IActionResult> ModifyBettingLimitAsync(ModifyBettingLimitCommand command)
    {
        var result = await Mediator.Send(command);
        return ApiResponseFactory.Ok(result);
    }
}