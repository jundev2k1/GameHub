using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Identity;
using game_x.application.Events.OnUserLogin;

namespace game_x.application.Features.Auth.Client.Commands.UserLogin;

public sealed class UserLoginHandler(
    IJwtTokenGenerator jwtTokenGenerator,
    IAuthService authService,
    IApplicationEventDispatcher eventDispatcher) : ICommandHandler<UserLoginCommand, UserLoginResult>
{
    public async Task<UserLoginResult> Handle(UserLoginCommand request, CancellationToken ct = default)
    {
        var loginUser = await authService.TryLoginAsync(request.Email, request.Password);
        var (isValid, errorCode) = loginUser.CheckValidUser();
        if (!isValid) throw new ForbiddenException(errorCode!);

        var roles = await authService.GetRolesAsync(loginUser);
        if (!roles.IsUser) throw new ForbiddenException();

        if(!loginUser.EmailConfirmed)
            throw new BadRequestException(MessageCode.User.UserNotConfirmed);
        
        await eventDispatcher.Publish(new OnUserLoginEvent(loginUser.Id), ct);
        
        var tokenInfo = await jwtTokenGenerator.GenerateToken(loginUser);
        return new UserLoginResult(
            Email: loginUser.Email!,
            UserId: loginUser.Id,
            Nickname: loginUser.Nickname,
            Token: tokenInfo.Token,
            ExpiresAt: tokenInfo.ExpiresAt,
            Roles: roles.Items);
    }
}
