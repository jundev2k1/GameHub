using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Identity;
using game_x.application.Features.Auth.Dtos;

namespace game_x.application.Features.Auth.Commands.Login.AdminLogin;

public sealed class AdminLoginHandler(
    IJwtTokenGenerator jwtTokenGenerator,
    IAuthService authService) : ICommandHandler<AdminLoginCommand, AdminLoginDto>
{
    public async Task<AdminLoginDto> Handle(AdminLoginCommand request, CancellationToken ct)
    {
        var user = await authService.TryLoginAsync(request.UserName, request.Password);
        var (isValid, errorCode) = user.CheckValidUser();
        if (!isValid) throw new ForbiddenException(errorCode!);

        var roles = await authService.GetRolesAsync(user);
        if (!roles.IsAdmin) throw new ForbiddenException();

        var tokenInfo = await jwtTokenGenerator.GenerateToken(user);
        return new AdminLoginDto
        {
            Token = tokenInfo.Token,
            ExpiresAt = tokenInfo.ExpiresAt,
            UserId = user.Id,
            UserName = user.UserName ?? string.Empty,
            Roles = [.. roles]
        };
    }
}
