using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;
using game_x.application.Features.Auth.Dtos;
using game_x.share.Helper;
using Microsoft.Extensions.Caching.Memory;

namespace game_x.infrastructure.Caching;

public sealed class RefreshTokenManagerCacheService(
    IMemoryCache cache,
    IRefreshTokenRepo refreshTokenRepo) : CacheService(cache), IRefreshTokenManagerCacheService
{
    private const string CacheKeyPrefix = "RefreshTokenManager";

    private RefreshTokenDto[] GetValidRefreshTokens()
    {
        var validTokens = refreshTokenRepo.GetActiveTokensAsync()
            .Result
            .Select(token => token.Adapt<RefreshToken, RefreshTokenDto>())
            .ToArray();
        Set($"{CacheKeyPrefix}:list", validTokens);
        return validTokens;
    }

    public RefreshTokenDto[] GetAllTokens() => DataSource;

    public RefreshTokenDto GetToken(string rawToken)
    {
        return DataSource
            .FirstOrDefault(rt => rt.TokenHash == HashHelper.Sha256(rawToken))
            ?? throw new NotFoundException("Refresh Token is invalid or not found.");
    }

    public void InsertNewToken(RefreshTokenDto tokenDto)
    {
        DataSource = [.. DataSource, tokenDto];
    }

    public void RevokeToken(string tokenHash)
    {
        var tokens = DataSource;
        var tokenToRevoke = tokens.FirstOrDefault(rt => rt.TokenHash == tokenHash && rt.RevokedAt is null)
            ?? throw new NotFoundException("Token not found or has been revoked");

        tokenToRevoke.RevokedAt = DateTime.UtcNow;
        DataSource = [.. tokens];
    }

    public void ReplaceToken(string oldTokenHash, string newTokenHash)
    {
        var tokens = DataSource;
        var tokenToReplace = tokens.FirstOrDefault(rt => rt.TokenHash == oldTokenHash && rt.RevokedAt is null)
            ?? throw new NotFoundException("Token not found or has been revoked");

        tokenToReplace.ReplacedByToken = newTokenHash;
        DataSource = [.. tokens];
    }

    private RefreshTokenDto[] DataSource
    {
        get { return Get<RefreshTokenDto[]>($"{CacheKeyPrefix}:list") ?? GetValidRefreshTokens(); }
        set { Set($"{CacheKeyPrefix}:list", value); }
    }
}
