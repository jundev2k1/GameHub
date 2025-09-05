using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Exceptions;
using game_x.application.Features.Accounts.User.Commands.RevokeAllOtherToken;
using game_x.application.Features.Accounts.User.Commands.RevokeToken;
using game_x.application.Features.Accounts.User.Commands.UserSelfUpdate;
using game_x.application.Features.Accounts.User.Queries.GetAllActiveTokens;
using game_x.application.Features.Accounts.User.Queries.GetSelfUser;
using game_x.application.Features.Accounts.User.Queries.GetSelfUserBalance;
using game_x.application.Features.Accounts.User.Queries.GetSelfVerificationStatusList;
using game_x.application.Features.Auth.Client.Commands.ChangePasswordUser;

namespace game_x.api.Controllers.Client.Me;

[Authorize(Roles = AppRoles.User)]
[Route("api/user/me")]
public sealed class UserController(
    IUserAccessor userAccessor,
    IRefreshTokenManagerCacheService refreshTokenManager) : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetUserDetailAsync()
    {
        var result = await Mediator.Send(new GetSelfUserQuery());
        return ApiResponseFactory.Ok(result);
    }

    [HttpGet("verification-statuses")]
    public async Task<IActionResult> GetUserVerificationListAsync()
    {
        var result = await Mediator.Send(new GetSelfVerificationStatusListQuery());
        return ApiResponseFactory.Ok(result);
    }

    [HttpPatch("password")]
    public async Task<IActionResult> ChangePasswordAsync(ChangePasswordUserCommand command)
    {
        if (command.OldPassword.Trim() == command.NewPassword.Trim())
            throw new BadRequestException();

        await Mediator.Send(command);
        return ApiResponseFactory.NoContent(MessageCode.User.UserChangePasswordSuccess);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateUserAsync(UserSelfUpdateCommand command)
    {
        await Mediator.Send(command);
        return ApiResponseFactory.NoContent(code: MessageCode.System.Updated);
    }

    [HttpGet("balances")]
    public async Task<IActionResult> GetUserBalanceAsync()
    {
        var result = await Mediator.Send(new GetSelfUserBalanceQuery());
        return ApiResponseFactory.Ok(result);
    }

    [HttpGet("tokens")]
    public async Task<IActionResult> GetActiveTokensAsync()
    {
        var query = new GetAllActiveTokensQuery();
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }

    [HttpDelete("tokens/{tokenId}")]
    public async Task<IActionResult> RevokeTokenAsync(Guid tokenId)
    {
        var userId = userAccessor.GetUserId();
        var currentJwtId = userAccessor.GetJwtId();
        var tokens = refreshTokenManager.GetsByUserId(userId!);
        var targetToken = tokens.FirstOrDefault(t => t.PublicId == tokenId);

        if (targetToken!.JwtId == currentJwtId)
            return ApiResponseFactory.Forbidden(MessageCode.System.Forbidden);

        var command = new RevokeTokenCommand(userId!, tokenId);
        await Mediator.Send(command);
        return ApiResponseFactory.NoContent();
    }

    [HttpDelete("tokens/others")]
    public async Task<IActionResult> RevokeOtherTokensAsync()
    {
        await Mediator.Send(new RevokeAllOtherTokenCommand());
        return ApiResponseFactory.NoContent();
    }
}
