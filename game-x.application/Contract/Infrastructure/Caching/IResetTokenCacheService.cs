namespace game_x.application.Contract.Infrastructure.Caching;

public interface IResetTokenCacheService
{
    void StoreToken(string token, string email, TimeSpan? expiresIn = null);

    string? GetEmailByToken(string token);

    void InvalidateToken(string token);
}
