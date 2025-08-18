using game_x.application.Exceptions;
using game_x.application.Features.Accounts.User.Commands.RevokeToken;
using game_x.application.Features.Accounts.User.Commands.UserSelfUpdate;
using game_x.application.Features.Accounts.User.Queries.GetSelfUser;
using game_x.application.Features.Accounts.User.Queries.GetSelfUserBalance;
using game_x.application.Features.Accounts.User.Queries.GetSelfVerificationStatusList;
using game_x.application.Features.Auth.Client.Commands.ChangePasswordUser;

namespace game_x.api.Controllers.Client.Me;

[Authorize(Roles = AppRoles.User)]
[Route("api/user/me")]
public sealed class UserController : BaseApiController
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

    [HttpDelete("tokens")]
    public async Task<IActionResult> RevokeTokenAsync(RevokeTokenCommand command)
    {
        await Mediator.Send(command);
        return ApiResponseFactory.NoContent();
    }
}
