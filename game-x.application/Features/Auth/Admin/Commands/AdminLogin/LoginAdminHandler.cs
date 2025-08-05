using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Identity;

namespace game_x.application.Features.Auth.Admin.Commands.AdminLogin;

public sealed class LoginAdminHandler(
    IJwtTokenGenerator jwtTokenGenerator,
    IAuthService authService) : ICommandHandler<LoginAdminCommand, AdminLoginResult>
{
    public async Task<AdminLoginResult> Handle(LoginAdminCommand request, CancellationToken ct)
    {
        var loginUser = await authService.TryLoginAsync(request.UserName, request.Password);
        var (isValid, errorCode) = loginUser.CheckValidUser();
        if (!isValid) throw new ForbiddenException(errorCode!);

        var roles = await authService.GetRolesAsync(loginUser);
        if (!roles.IsAdmin) throw new ForbiddenException();

        var tokenInfo = await jwtTokenGenerator.GenerateToken(loginUser);
        return new AdminLoginResult(
            UserName: loginUser.UserName!,
            UserId: loginUser.Id,
            Token: tokenInfo.Token,
            ExpiresAt: tokenInfo.ExpiresAt,
            Roles: roles.Items);
    }
}
