namespace game_x.application.Contract.Infrastructure.Caching;

public interface IGameProviderCacheService
{
    void SetLanguage(string account, string language);

    string GetLanguage(string account);

    bool GetIsLoggedIn(string account);

    void SetIsLoggedIn(string account, bool isLoggedIn);
}
