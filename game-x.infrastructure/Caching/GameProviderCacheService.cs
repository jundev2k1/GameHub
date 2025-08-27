using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;
using game_x.application.Features.Games.Dtos;
using Microsoft.Extensions.Caching.Memory;

namespace game_x.infrastructure.Caching;

public sealed class GameProviderCacheService(
    IMemoryCache cache,
    IGamePlatformRepo gamePlatformRepo,
    IGameCategoryRepo gameCategoryRepo,
    IGameTypeRepo gameTypeRepo,
    IGameRepo gameRepo)
    : CacheService(cache), IGameProviderCacheService
{
    private readonly string _prefixCache = "external:game-provider:";

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
        var gameList = await gamePlatformRepo.GetAllAsync();
        Set(cacheKey, gameList.Select(g => g.Adapt<GamePlatformDto>()).ToArray());
    }

    public async Task RefreshGameCategoryList()
    {
        var cacheKey = $"{_prefixCache}:category:list";
        var gameList = await gameCategoryRepo.GetAllAsync();
        Set(cacheKey, gameList.Select(g => g.Adapt<GameCategoryDto>()).ToArray());
    }

    public async Task RefreshGameTypeList()
    {
        var cacheKey = $"{_prefixCache}:game-type:list";
        var gameList = await gameTypeRepo.GetAllAsync();
        Set(cacheKey, gameList.Select(g => g.Adapt<GameTypeDto>()).ToArray());
    }

    public async Task RefreshGameList()
    {
        var cacheKey = $"{_prefixCache}:game:list";
        var gameList = await gameRepo.GetAllAsync();
        Set(cacheKey, gameList.Select(g => g.Adapt<GameInfoDto>()).ToArray());
    }

    public GamePlatformDto[] PlatformList
        => Get<GamePlatformDto[]>($"{_prefixCache}:platform:list") ?? [];
    public GameCategoryDto[] CategoryList
        => Get<GameCategoryDto[]>($"{_prefixCache}:category:list") ?? [];
    public GameTypeDto[] GameTypeList
        => Get<GameTypeDto[]>($"{_prefixCache}:game-type:list") ?? [];
    public GameInfoDto[] GameList
        => Get<GameInfoDto[]>($"{_prefixCache}:game:list") ?? [];

    public GamePlatformDto G598Platform
        => PlatformList.FirstOrDefault(p => p.Id == Guid.Parse("b2e3c5bb-6b74-4bb0-9dc3-9e8a1e70d94a"))
        ?? throw new NotFoundException();
}
