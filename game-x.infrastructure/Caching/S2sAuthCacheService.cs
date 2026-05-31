using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;
using game_x.application.Features.S2s.DTOs;
using Microsoft.Extensions.Caching.Memory;

namespace game_x.infrastructure.Caching;

public sealed class S2sAuthCacheService(
    IMemoryCache cache,
    IS2sCredentialRepo s2SCredentialRepo) : CacheService(cache), IS2sAuthCacheService
{
    private const string CachePrefix = "S2sAuth:";
    private const int ExpiredMinutes = 3;

    private async Task RefreshAsync(string keyId, AuthMethod method)
    {
        var credentials = await s2SCredentialRepo.GetByKeyIdAsync(keyId);
        var dto = S2sCredentialCacheDto.Create(credentials.Adapt<S2sCredentialDto[]>())
            ?? throw new NotFoundException(nameof(keyId), keyId);

        if (dto.AuthMethod != method)
            throw new NotFoundException(nameof(method), method);

        var cacheKey = $"{CachePrefix}{keyId}:{method}";
        Set(cacheKey, dto, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(ExpiredMinutes)
        });
    }

    public async Task<S2sCredentialDto> GetAsync(string keyId, AuthMethod method)
    {
        var cacheKey = $"{CachePrefix}{keyId}:{method}";
        var data = Get<S2sCredentialDto?>(cacheKey);
        if (data != null) return data;

        await RefreshAsync(keyId, method);
        return Get<S2sCredentialDto>(cacheKey)!;
    }
}
