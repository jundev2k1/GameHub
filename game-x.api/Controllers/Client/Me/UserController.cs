using game_x.application.Common.Files;
using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Exceptions;
using game_x.application.Features.Accounts.User.Commands.RevokeAllOtherToken;
using game_x.application.Features.Accounts.User.Commands.RevokeToken;
using game_x.application.Features.Accounts.User.Commands.UploadAvatar;
using game_x.application.Features.Accounts.User.Commands.UserSelfUpdate;
using game_x.application.Features.Accounts.User.Queries.GetAllActiveTokens;
using game_x.application.Features.Accounts.User.Queries.GetSelfUser;
using game_x.application.Features.Accounts.User.Queries.GetSelfUserBalance;
using game_x.application.Features.Accounts.User.Queries.GetSelfVerificationStatusList;
using game_x.application.Features.Auth.Client.Commands.ChangePasswordUser;

namespace game_x.api.Controllers.Client.Me;

[Route("api/user/me")]
public sealed class UserController(
    IUserAccessor userAccessor,
    IRefreshTokenManagerCacheService refreshTokenManager) : BaseApiController
{
    [Authorize(Roles = $"{AppRoles.Talent},{AppRoles.User}")]
    [HttpGet]
    public async Task<IActionResult> GetUserDetailAsync()
    {
        var result = await Mediator.Send(new GetSelfUserQuery());
        return ApiResponseFactory.Ok(result);
    }

    [Authorize(Roles = AppRoles.User)]
    [HttpGet("verification-statuses")]
    public async Task<IActionResult> GetUserVerificationListAsync()
    {
        var result = await Mediator.Send(new GetSelfVerificationStatusListQuery());
        return ApiResponseFactory.Ok(result);
    }

    [Authorize(Roles = AppRoles.User)]
    [HttpPatch("password")]
    public async Task<IActionResult> ChangePasswordAsync(ChangePasswordUserCommand command)
    {
        if (command.OldPassword.Trim() == command.NewPassword.Trim())
            throw new BadRequestException();

        await Mediator.Send(command);
        return ApiResponseFactory.NoContent(MessageCode.User.UserChangePasswordSuccess);
    }

    [Authorize(Roles = AppRoles.User)]
    [HttpPut]
    public async Task<IActionResult> UpdateUserAsync(UserSelfUpdateCommand command)
    {
        await Mediator.Send(command);
        return ApiResponseFactory.NoContent(code: MessageCode.System.Updated);
    }

    [Authorize(Roles = AppRoles.User)]
    [HttpGet("balances")]
    public async Task<IActionResult> GetUserBalanceAsync()
    {
        var result = await Mediator.Send(new GetSelfUserBalanceQuery());
        return ApiResponseFactory.Ok(result);
    }

    [Authorize(Roles = $"{AppRoles.Talent},{AppRoles.User}")]
    [HttpGet("tokens")]
    public async Task<IActionResult> GetActiveTokensAsync()
    {
        var query = new GetAllActiveTokensQuery();
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }

    [Authorize(Roles = $"{AppRoles.Talent},{AppRoles.User}")]
    [HttpDelete("tokens/{tokenId}")]
    public async Task<IActionResult> RevokeTokenAsync(Guid tokenId)
    {
        var userId = userAccessor.GetUserId();
        var currentJwtId = userAccessor.GetJwtId();
        var tokens = refreshTokenManager.GetsByUserId(userId);
        var targetToken = tokens.FirstOrDefault(t => t.PublicId == tokenId);

        if (targetToken!.JwtId == currentJwtId)
            return ApiResponseFactory.Forbidden(MessageCode.System.Forbidden);

        var command = new RevokeTokenCommand(userId, tokenId);
        await Mediator.Send(command);
        return ApiResponseFactory.NoContent();
    }

    [Authorize(Roles = $"{AppRoles.Talent},{AppRoles.User}")]
    [HttpDelete("tokens/others")]
    public async Task<IActionResult> RevokeOtherTokensAsync()
    {
        await Mediator.Send(new RevokeAllOtherTokenCommand());
        return ApiResponseFactory.NoContent();
    }

    [Authorize(Roles = $"{AppRoles.Talent},{AppRoles.User}")]
    [HttpPost("avatar")]
    public async Task<IActionResult> UploadAvatarAsync(IFormFile file)
    {
        var result = await Mediator.Send(new UploadAvatarCommand(FileUpload.FromFormFile(file)));
        return ApiResponseFactory.Ok(result);
    }
}
