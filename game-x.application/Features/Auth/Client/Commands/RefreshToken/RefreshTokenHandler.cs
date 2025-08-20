using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Auth.Dtos;
using game_x.share.Helper;
using RefreshTokenEntity = game_x.domain.Entities.RefreshToken;

namespace game_x.application.Features.Auth.Client.Commands.RefreshToken;

public sealed class RefreshTokenHandler(
    IRefreshTokenManagerCacheService refreshTokenManager,
    IUserAccessor userAccessor,
    IJwtTokenGenerator tokenGenerator,
    ITokenService tokenService,
    IUserRepo userRepo) : ICommandHandler<RefreshTokenCommand, RefreshTokenResult>
{
    public async Task<RefreshTokenResult> Handle(RefreshTokenCommand request, CancellationToken ct = default)
    {
        var tokenInfo = tokenGenerator.DecodeToken(request.AccessToken);
        var currentRefreshToken = refreshTokenManager.GetToken(tokenInfo.Subject!, request.RefreshToken);

        var isValid = IsValidToken(tokenInfo, currentRefreshToken);
        if (!isValid) throw new BadRequestException(MessageCode.System.InvalidOrMissingToken);

        var (accessToken, refreshToken) = await CreateToken(currentRefreshToken);
        return new RefreshTokenResult(accessToken, refreshToken);
    }

    private static bool IsValidToken(JwtPayloadDto tokenInfo, RefreshTokenDto currentRefreshToken)
    {
        // 1. JWT ID check
        if (tokenInfo.JwtId != currentRefreshToken.JwtId)
            return false;

        // 2. Access token check: the token must be expired
        if (!tokenInfo.IsExpired)
            return false;

        // 3. Refresh token must be valid
        if (currentRefreshToken.IsExpired || currentRefreshToken.IsRevoked)
            return false;

        // 4. Prevent reuse chain
        if (!string.IsNullOrEmpty(currentRefreshToken.ReplacedByToken))
            return false;

        return true;
    }

    private async Task<(string AccessToken, string RefreshToken)> CreateToken(RefreshTokenDto oldRefreshToken)
    {
        var targetUser = await userRepo.GetUserByIdAsync(oldRefreshToken.UserId);
        var newAccessToken = await tokenGenerator.GenerateToken(targetUser);
        var newRefreshToken = tokenService.GenerateRefreshToken(targetUser.Id);

        var refreshTokenEntity = RefreshTokenEntity.Create(
            targetUser.Id,
            HashHelper.Sha256(newRefreshToken.Token),
            newAccessToken.JwtId,
            newRefreshToken.ExpiresAt,
            userAccessor.GetIpAddress(),
            userAccessor.GetUserAgent(),
            deviceInfo: userAccessor.GetDeviceInfo());
        var refreshTokenDto = refreshTokenEntity.Adapt<RefreshTokenDto>();
        refreshTokenManager.InsertNewToken(refreshTokenDto);
        refreshTokenManager.ReplaceToken(
            oldRefreshToken.UserId,
            oldRefreshToken.TokenHash,
            refreshTokenDto.TokenHash);

        return (newAccessToken.Token, newRefreshToken.Token);
    }
}
