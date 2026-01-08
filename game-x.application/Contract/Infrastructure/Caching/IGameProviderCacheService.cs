using game_x.application.Features.Games.Dtos;

namespace game_x.application.Contract.Infrastructure.Caching;

public interface IGameProviderCacheService
{
    void SetLanguage(string account, string language);

    string GetLanguage(string account);

    bool GetIsLoggedIn(string account);

    void SetIsLoggedIn(string account, bool isLoggedIn);

    Task RefreshGamePlatformList();

    Task RefreshGameCategoryList();

    Task RefreshGameTypeList();

    Task RefreshGameTagList();

    Task RefreshGameRecommendList();

    Task RefreshGameList();

    Task<string> GetGameThumbnail(GameInfoDto game);

    (string? token, DateTime? expiredTime) GetProviderToken(Guid platformId);

    void SetProviderToken(Guid platformId, string token, DateTime expiredTime);
  
    GamePlatformDto[] PlatformList { get; }
    GameCategoryDto[] CategoryList { get; }
    GameTypeDto[] GameTypeList { get; }
    GameTagDto[] GameTagList { get; }
    GameRecommendDto[] GameRecommendList { get; }
    GameInfoDto[] GameList { get; }

    GamePlatformDto G598Platform { get; }
    GamePlatformDto BaccaratPlatform { get; }
    GamePlatformDto Etl998Platform { get; }
    GamePlatformDto SasSlotPlatform { get; }
    GamePlatformDto AtgPlatform { get; }
}