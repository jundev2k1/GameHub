using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Identity;
using game_x.application.Features.Auth.Dtos;
using game_x.share.Helper;
using RefreshTokenEntity = game_x.domain.Entities.RefreshToken;

namespace game_x.application.Features.Auth.Root.Commands.RootLogin;

public sealed class RootLoginHandler(
    IJwtTokenGenerator jwtTokenGenerator,
    ITokenService tokenService,
    IUserAccessor userAccessor,
    IRefreshTokenManagerCacheService refreshTokenManager,
    IAuthService authService) : ICommandHandler<RootLoginCommand, RootLoginResult>
{
    public async Task<RootLoginResult> Handle(RootLoginCommand request, CancellationToken ct)
    {
        var user = await authService.TryLoginAsync(request.UserName, request.Password);
        var role = await authService.GetRolesAsync(user);
        if (!role.IsRoot)
            throw new ForbiddenException();

        var tokenInfo = await jwtTokenGenerator.GenerateToken(user);
        var refreshToken = tokenService.GenerateRefreshToken(user.Id);
        CreateRefreshToken(user.Id, refreshToken, tokenInfo.JwtId);

        return new RootLoginResult(tokenInfo.Token, refreshToken.Token);
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
