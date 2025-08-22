using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Identity;
using game_x.application.Features.Auth.Dtos;
using game_x.share.Helper;
using RefreshTokenEntity = game_x.domain.Entities.RefreshToken;

namespace game_x.application.Features.Auth.Admin.Commands.AdminLogin;

public sealed class LoginAdminHandler(
    IJwtTokenGenerator jwtTokenGenerator,
    ITokenService tokenService,
    IUserAccessor userAccessor,
    IRefreshTokenManagerCacheService refreshTokenManager,
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
        var refreshToken = tokenService.GenerateRefreshToken(loginUser.Id);
        CreateRefreshToken(loginUser.Id, refreshToken, tokenInfo.JwtId);
        refreshTokenManager.RevokeAllTokenSameDevice(loginUser.Id, refreshToken.Token);

        return new AdminLoginResult(
            UserName: loginUser.UserName!,
            UserId: loginUser.Id,
            AccessToken: tokenInfo.Token,
            RefreshToken: refreshToken.Token);
    }

    private void CreateRefreshToken(string userId, RefreshTokenGenerateDto tokenInfo, string jwtId)
    {
        var token = RefreshTokenEntity.Create(
            userId,
            HashHelper.Sha256(tokenInfo.Token),
            jwtId,
            tokenInfo.ExpiresAt,
            userAccessor.GetIpAddress(),
            userAccessor.GetUserAgent(),
            deviceInfo: userAccessor.GetDeviceInfo());
        refreshTokenManager.InsertNewToken(token.Adapt<RefreshTokenDto>());
    }
}
