using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.FileStorage;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;
using game_x.application.Features.Games.Dtos;
using game_x.share.Extensions;
using Microsoft.Extensions.Caching.Memory;

namespace game_x.infrastructure.Caching;

public sealed class GameProviderCacheService(
    IMemoryCache cache,
    IGamePlatformRepo gamePlatformRepo,
    IGameCategoryRepo gameCategoryRepo,
    IGameTypeRepo gameTypeRepo,
    IGameTagRepo gameTagRepo,
    IGameRecommendRepo gameRecommendRepo,
    IGameRepo gameRepo,
    IFileStorageService fileStorage)
    : CacheService(cache), IGameProviderCacheService
{
    private readonly string _prefixCache = "external:game-provider";

    public string GetLanguage(string account)
    {
        var language = Get<string>($"{_prefixCache}:{account}:language") ?? "zh-Hant";
        return language;
    }

    public void SetLanguage(string account, string language)
    {
        Set($"{_prefixCache}:{account}:language", language);
    }

    public bool GetIsLoggedIn(string account)
    {
        var isLoggedIn = Get<bool>($"{_prefixCache}:{account}:is-logged-in");
        return isLoggedIn;
    }

    public void SetIsLoggedIn(string account, bool isLoggedIn)
    {
        Set($"{_prefixCache}:{account}:is-logged-in", isLoggedIn);
    }

    public async Task RefreshGamePlatformListAsync(CancellationToken ct = default)
    {
        var cacheKey = $"{_prefixCache}:platform:list";
        var gamePlatformList = await gamePlatformRepo.GetAllAsync(ct);
        Set(cacheKey, gamePlatformList.Select(g => g.Adapt<GamePlatformDto>()).ToArray());
    }

    public async Task RefreshGameCategoryListAsync(CancellationToken ct = default)
    {
        var cacheKey = $"{_prefixCache}:category:list";
        var gameCategoryList = await gameCategoryRepo.GetAllAsync(ct);
        Set(cacheKey, gameCategoryList.Select(g => g.Adapt<GameCategoryDto>()).ToArray());
    }

    public async Task RefreshGameTypeListAsync(CancellationToken ct = default)
    {
        var cacheKey = $"{_prefixCache}:game-type:list";
        var gameTypeList = await gameTypeRepo.GetAllAsync(ct);
        Set(cacheKey, gameTypeList.Select(g => g.Adapt<GameTypeDto>()).ToArray());
    }

    public async Task RefreshGameTagListAsync(CancellationToken ct = default)
    {
        var cacheKey = $"{_prefixCache}:game-tag:list";
        var gameTagList = await gameTagRepo.GetAllAsync(ct);
        Set(cacheKey, gameTagList.Select(g => g.Adapt<GameTagDto>()).ToArray());
    }

    public async Task RefreshGameRecommendListAsync(CancellationToken ct = default)
    {
        var cacheKey = $"{_prefixCache}:game-recommend:list";
        var dto = await gameRecommendRepo.GetAllAsync(ct);
        Set(cacheKey, dto);
    }

    public async Task RefreshGameListAsync(CancellationToken ct = default)
    {
        var gameList = await gameRepo.GetAllAsync(ct);

        // Clear all game thumbnail caches
        foreach (var game in gameList)
        {
            var cacheThumbnailKey = $"{_prefixCache}:game:{game.PublicId}:thumbnail";
            Remove(cacheThumbnailKey);
        }

        var cacheKey = $"{_prefixCache}:game:list";
        Set(cacheKey, gameList.Adapt<GameInfoDto[]>());
    }

    public async Task<string> GetGameThumbnailAsync(GameInfoDto game)
    {
        // No thumbnail
        if (game.Thumbnail is null) return string.Empty;

        // Get thumbnail from cache
        var cacheKey = $"{_prefixCache}:game:{game.Id}:thumbnail";
        var thumbnailUrl = Get<string>(cacheKey);
        if (thumbnailUrl.IsNotNullOrEmpty()) return thumbnailUrl!;

        // Generate new thumbnail url and cache it
        var url = await fileStorage.GenerateDownloadUrlAsync(
            BucketName.Of(game.Thumbnail.BucketName),
            ObjectName.Of(game.Thumbnail.ObjectName),
            TimeSpan.FromHours(8));
        Set(cacheKey, thumbnailUrl!, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(8)
        });
        return url;
    }

    public (string? token, DateTime? expiredTime) GetProviderToken(Guid platformId)
    {
        var token = Get<string>($"{_prefixCache}:{platformId}:provider-token");
        var expiredTime = Get<DateTime?>($"{_prefixCache}:{platformId}:provider-token-expired-time");
        return (token, expiredTime);
    }

    public void SetProviderToken(Guid platformId, string token, DateTime expiredTime)
    {
        Set($"{_prefixCache}:{platformId}:provider-token", token);
        Set($"{_prefixCache}:{platformId}:provider-token-expired-time", expiredTime);
    }
    
    public GamePlatformDto[] PlatformList
        => Get<GamePlatformDto[]>($"{_prefixCache}:platform:list") ?? [];
    public GameCategoryDto[] CategoryList
        => Get<GameCategoryDto[]>($"{_prefixCache}:category:list") ?? [];
    public GameTypeDto[] GameTypeList
        => Get<GameTypeDto[]>($"{_prefixCache}:game-type:list") ?? [];
    public GameTagDto[] GameTagList
        => Get<GameTagDto[]>($"{_prefixCache}:game-tag:list") ?? [];
    public GameRecommendDto[] GameRecommendList
        => Get<GameRecommendDto[]>($"{_prefixCache}:game-recommend:list") ?? [];
    public GameInfoDto[] GameList
        => Get<GameInfoDto[]>($"{_prefixCache}:game:list") ?? [];

    public GamePlatformDto G598Platform
        => PlatformList.FirstOrDefault(p => p.Id == GameConstants.PLATFORM_ID_G598)
        ?? throw new NotFoundException();

    public GamePlatformDto BaccaratPlatform
        => PlatformList.FirstOrDefault(p => p.Id == GameConstants.PLATFORM_ID_GAMEBACCARAT)
            ?? throw new NotFoundException();

    public GamePlatformDto Etl998Platform
        => PlatformList.FirstOrDefault(p => p.Id == GameConstants.PLATFORM_ID_ETL998_GAMEBACCARAT)
           ?? throw new NotFoundException();

    public GamePlatformDto SasSlotPlatform
        => PlatformList.FirstOrDefault(p => p.Id == GameConstants.PLATFORM_ID_SASSLOT)
           ?? throw new NotFoundException();
    
    public GamePlatformDto AtgPlatform
        => PlatformList.FirstOrDefault(p => p.Id == GameConstants.PLATFORM_ID_ATG)
           ?? throw new NotFoundException();
}