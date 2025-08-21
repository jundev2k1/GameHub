using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Identity;
using game_x.application.Features.Auth.Dtos;
using game_x.share.Helper;
using RefreshTokenEntity = game_x.domain.Entities.RefreshToken;

namespace game_x.application.Features.Auth.Cs.Commands.CsLogin;

public sealed class CsLoginHandler(
    IAuthService authService,
    ITokenService tokenService,
    IUserAccessor userAccessor,
    IRefreshTokenManagerCacheService refreshTokenManager,
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
        var refreshToken = tokenService.GenerateRefreshToken(loginUser.Id);
        CreateRefreshToken(loginUser.Id, refreshToken, tokenInfo.JwtId);
        refreshTokenManager.RevokeAllTokenSameDevice(loginUser.Id, refreshToken.Token);

        return new CsLoginResult(
            UserId: loginUser.Id,
            Username: loginUser.UserName!,
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
