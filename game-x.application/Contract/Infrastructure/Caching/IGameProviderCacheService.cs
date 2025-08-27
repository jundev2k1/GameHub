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

    Task RefreshGameList();

    GamePlatformDto[] PlatformList { get; }
    GameCategoryDto[] CategoryList { get; }
    GameTypeDto[] GameTypeList { get; }
    GameInfoDto[] GameList { get; }

    GamePlatformDto G598Platform { get; }
}