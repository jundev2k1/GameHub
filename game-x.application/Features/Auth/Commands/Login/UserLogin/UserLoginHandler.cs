using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Identity;
using game_x.application.Features.Auth.Dtos;

namespace game_x.application.Features.Auth.Commands.Login.UserLogin;

public sealed class UserLoginHandler(
    IJwtTokenGenerator jwtTokenGenerator,
    IAuthService authService) : ICommandHandler<UserLoginCommand, UserLoginDto>
{
    public async Task<UserLoginDto> Handle(UserLoginCommand request, CancellationToken ct = default)
    {
        var loginUser = await authService.TryLoginAsync(request.UserName, request.Password);
        var (isValid, errorCode) = loginUser.CheckValidUser();
        if (!isValid) throw new ForbiddenException(errorCode!);

        var roles = await authService.GetRolesAsync(loginUser);
        if (!roles.IsUser) throw new ForbiddenException();

        var tokenInfo = await jwtTokenGenerator.GenerateToken(loginUser);
        return new UserLoginDto
        {
            Token = tokenInfo.Token,
            ExpiresAt = tokenInfo.ExpiresAt,
            UserId = loginUser.Id,
            UserName = loginUser.UserName ?? string.Empty,
            Roles = [.. roles]
        };
    }
}
