using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;
using game_x.application.Features.Auth.Dtos;
using game_x.share.Helper;
using Microsoft.Extensions.Caching.Memory;

namespace game_x.infrastructure.Caching;

public sealed class RefreshTokenManagerCacheService(
    IMemoryCache cache,
    IUserAccessor userAccessor,
    IRefreshTokenRepo refreshTokenRepo) : CacheService(cache), IRefreshTokenManagerCacheService
{
    private const string CacheKeyPrefix = "RefreshTokenManager";

    private Dictionary<string, Guid[]> GetValidRefreshTokens()
    {
        static RefreshTokenDto MapToDtoFromDb(RefreshToken token)
        {
            var dto = token.Adapt<RefreshTokenDto>();
            dto.State = SyncState.Synced;
            return dto;
        }

        var validTokens = refreshTokenRepo.GetActiveTokensAsync()
            .Result
            .Select(MapToDtoFromDb)
            .ToList();

        // Group tokens by UserId and create a dictionary, insert into cache
        var dataSource = validTokens
            .GroupBy(rt => rt.UserId)
            .ToDictionary(
                gr => gr.Key,
                gr => gr.Select(rt => rt.PublicId).ToArray());
        DataSource = dataSource;

        // Insert valid tokens into cache
        validTokens.ForEach(InsertNewToken);

        return dataSource;
    }

    public IEnumerable<RefreshTokenDto> GetAllTokens()
    {
        foreach (var kvp in DataSource)
        {
            var userId = kvp.Key;
            foreach (var tokenId in kvp.Value)
            {
                var cacheKey = $"{CacheKeyPrefix}:{userId}:{tokenId}";
                var token = Get<RefreshTokenDto>(cacheKey);
                if (token is null) continue;

                yield return token;
            }
        }
    }

    public IEnumerable<RefreshTokenDto> GetsByUserId(string userId)
    {
        var targetIds = DataSource.FirstOrDefault(ds => ds.Key == userId).Value ?? [];
        foreach (var tokenId in targetIds)
        {
            var cacheKey = $"{CacheKeyPrefix}:{userId}:{tokenId}";
            var token = Get<RefreshTokenDto>(cacheKey);
            if (token is null) continue;

            yield return token;
        }
    }

    public RefreshTokenDto GetToken(string userId, string rawToken)
    {
        return GetsByUserId(userId)
            .FirstOrDefault(rt => rt.TokenHash == HashHelper.Sha256(rawToken))
            ?? throw new NotFoundException("Refresh Token is invalid or not found.");
    }

    public RefreshTokenDto? GetTokenByJwtId(string userId, string jwtId)
    {
        return GetsByUserId(userId)
            .FirstOrDefault(rt => rt.JwtId == jwtId);
    }

    private void SetNewValue(string cacheKey, RefreshTokenDto tokenDto)
    {
        Set(cacheKey, tokenDto, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = tokenDto.ExpiresAt.AddHours(1).TimeOfDay,
        });
    }

    public void InsertNewToken(RefreshTokenDto tokenDto)
    {
        // Set new item into cache
        var cacheKey = $"{CacheKeyPrefix}:{tokenDto.UserId}:{tokenDto.PublicId}";
        SetNewValue(cacheKey, tokenDto);

        // Get current tokens that group by user ID
        var tokens = DataSource;
        tokens.TryGetValue(tokenDto.UserId, out var groupIds);

        // Check if exist token id
        var isExistId = groupIds != null && groupIds.Contains(tokenDto.PublicId);
        if (isExistId) return;

        // Append new ID
        tokens[tokenDto.UserId] = [.. groupIds ?? [], tokenDto.PublicId];
        DataSource = tokens;
    }

    public void RevokeToken(string userId, string tokenHash)
    {
        // Check if the user exists in the cache
        var tokens = GetsByUserId(userId);
        var targetToken = tokens.FirstOrDefault(rt => rt.TokenHash == tokenHash && rt.RevokedAt is null)
            ?? throw new NotFoundException("Token not found or has been revoked");

        // Update the token state and revoked time
        RevokeToken(targetToken);
    }
    public void RevokeToken(string userId, Guid id)
    {
        // Check if the user exists in the cache
        var tokens = GetsByUserId(userId);
        var targetToken = tokens.FirstOrDefault(rt => rt.PublicId == id && rt.RevokedAt is null)
            ?? throw new NotFoundException("Token not found or has been revoked");

        // Update the token state and revoked time
        RevokeToken(targetToken);
    }
    public void RevokeToken(RefreshTokenDto token)
    {
        // Update the token state and revoked time
        token.State = token.State == SyncState.NotSynced
            ? token.State
            : SyncState.Updated;
        token.RevokedAt = DateTime.UtcNow;

        // Set the updated token back to cache
        var cacheKey = $"{CacheKeyPrefix}:{token.UserId}:{token.PublicId}";
        SetNewValue(cacheKey, token);
    }

    public void RevokeAllTokenSameDevice(string userId, string token)
    {
        var ipAddress = userAccessor.GetIpAddress();
        var deviceInfo = UserAgentHelper.GetDeviceKey(userAccessor.GetUserAgent());
        var hashToken = HashHelper.Sha256(token);
        var sameDeviceTokenIds = GetsByUserId(userId)
            .Where(rt => !rt.IsRevoked
                && !rt.IsExpired
                && rt.TokenHash != hashToken)
            .Select(rt => rt.TokenHash)
            .ToList();
        sameDeviceTokenIds.ForEach(
            token => RevokeToken(userId, token));
    }

    public void ReplaceToken(string userId, string oldTokenHash, string newTokenHash)
    {
        var tokens = GetsByUserId(userId);
        var tokenToReplace = tokens.FirstOrDefault(rt => rt.TokenHash == oldTokenHash && rt.RevokedAt is null)
            ?? throw new NotFoundException("Token not found or has been revoked");

        tokenToReplace.ReplacedByToken = newTokenHash;
        var cacheKey = $"{CacheKeyPrefix}:{userId}:{tokenToReplace.PublicId}";
        SetNewValue(cacheKey, tokenToReplace);
    }

    public void UpdateAfterSync(Guid[] tokenIds)
    {
        var updateTokens = GetAllTokens()
            .Where(rt => tokenIds.Contains(rt.PublicId));
        foreach (var token in updateTokens)
        {
            token.State = SyncState.Synced;
            var cacheKey = $"{CacheKeyPrefix}:{token.UserId}:{token.PublicId}";
            SetNewValue(cacheKey, token);
        }
    }

    public void RemoveExpiredTokens()
    {
        var expiredTokens = GetAllTokens()
            .Where(rt => rt.IsSynced && (rt.IsExpired || rt.IsRevoked))
            .ToArray();

        foreach (var token in expiredTokens)
        {
            var cacheKey = $"{CacheKeyPrefix}:{token.UserId}:{token.PublicId}";
            Remove(cacheKey);
        }

        var expiredTokenIds = expiredTokens
            .Select(rt => rt.PublicId)
            .ToArray();
        var tokens = DataSource.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.Where(id => expiredTokenIds.Contains(id) == false).ToArray());
        DataSource = tokens;
    }

    private Dictionary<string, Guid[]> DataSource
    {
        get { return Get<Dictionary<string, Guid[]>>($"{CacheKeyPrefix}:list") ?? GetValidRefreshTokens(); }
        set { Set($"{CacheKeyPrefix}:list", value); }
    }
}
