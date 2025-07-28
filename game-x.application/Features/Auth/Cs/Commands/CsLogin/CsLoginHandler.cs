using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Identity;

namespace game_x.application.Features.Auth.Cs.Commands.CsLogin;

public sealed class CsLoginHandler(
    IAuthService authService,
    IJwtTokenGenerator jwtTokenGenerator) : ICommandHandler<CsLoginCommand, CsLoginResult>
{
    public async Task<CsLoginResult> Handle(CsLoginCommand request, CancellationToken ct = default)
    {
        var loginUser = await authService.TryLoginAsync(request.UserName, request.Password);
        var (isValid, errorCode) = loginUser.CheckValidUser();
        if (!isValid) throw new ForbiddenException(errorCode!);

        var roles = await authService.GetRolesAsync(loginUser);
        if (!roles.IsCs) throw new ForbiddenException();

        var tokenInfo = await jwtTokenGenerator.GenerateToken(loginUser);
        return new CsLoginResult(
            UserName: loginUser.UserName!,
            UserId: loginUser.Id,
            Token: tokenInfo.Token,
            ExpiresAt: tokenInfo.ExpiresAt,
            Roles: roles.Items);
    }
}
