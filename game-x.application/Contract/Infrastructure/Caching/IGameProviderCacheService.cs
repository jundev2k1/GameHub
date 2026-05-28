using game_x.application.Features.Games.Dtos;

namespace game_x.application.Contract.Infrastructure.Caching;

public interface IGameProviderCacheService
{
    void SetLanguage(string account, string language);

    string GetLanguage(string account);

    bool GetIsLoggedIn(string account);

    void SetIsLoggedIn(string account, bool isLoggedIn);

    Task RefreshGamePlatformListAsync(CancellationToken ct = default);

    Task RefreshGameCategoryListAsync(CancellationToken ct = default);

    Task RefreshGameTypeListAsync(CancellationToken ct = default);

    Task RefreshGameTagListAsync(CancellationToken ct = default);

    Task RefreshGameRecommendListAsync(CancellationToken ct = default);

    Task RefreshGameListAsync(CancellationToken ct = default);

    Task RefreshSpecifyGameMediaAsync(Guid gameId, Guid? mediaId, CancellationToken ct = default);

    Task<string> GetGameThumbnailAsync(GameInfoDto game);

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