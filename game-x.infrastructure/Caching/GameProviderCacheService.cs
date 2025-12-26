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

    public async Task RefreshGamePlatformList()
    {
        var cacheKey = $"{_prefixCache}:platform:list";
        var gamePlatformList = await gamePlatformRepo.GetAllAsync();
        Set(cacheKey, gamePlatformList.Select(g => g.Adapt<GamePlatformDto>()).ToArray());
    }

    public async Task RefreshGameCategoryList()
    {
        var cacheKey = $"{_prefixCache}:category:list";
        var gameCategoryList = await gameCategoryRepo.GetAllAsync();
        Set(cacheKey, gameCategoryList.Select(g => g.Adapt<GameCategoryDto>()).ToArray());
    }

    public async Task RefreshGameTypeList()
    {
        var cacheKey = $"{_prefixCache}:game-type:list";
        var gameTypeList = await gameTypeRepo.GetAllAsync();
        Set(cacheKey, gameTypeList.Select(g => g.Adapt<GameTypeDto>()).ToArray());
    }

    public async Task RefreshGameTagList()
    {
        var cacheKey = $"{_prefixCache}:game-tag:list";
        var gameTagList = await gameTagRepo.GetAllAsync();
        Set(cacheKey, gameTagList.Select(g => g.Adapt<GameTagDto>()).ToArray());
    }

    public async Task RefreshGameRecommendList()
    {
        var cacheKey = $"{_prefixCache}:game-recommend:list";
        var gameRecommendList = await gameRecommendRepo.GetAllAsync();
        Set(cacheKey, gameRecommendList.Select(g => g.Adapt<GameRecommendDto>()).ToArray());
    }

    public async Task RefreshGameList()
    {
        var gameList = await gameRepo.GetAllAsync();

        // Clear all game thumbnail caches
        foreach (var game in gameList)
        {
            var cacheThumbnailKey = $"{_prefixCache}:game:{game.PublicId}:thumbnail";
            Remove(cacheThumbnailKey);
        }

        var cacheKey = $"{_prefixCache}:game:list";
        Set(cacheKey, gameList.Select(g => g.Adapt<GameInfoDto>()).ToArray());
    }

    public async Task<string> GetGameThumbnail(GameInfoDto game)
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
}