namespace game_x.application.Contract.Infrastructure.Caching;

public interface IUserQrCodeCacheService
{
    public string? GetUserId(string token);

    public void SetToken(string userId, string token, int? expiresIn = null);

    public void SetUserId(string token, string userId, int? expiresIn = null);

    public void RemoveToken(string userId);

    public void RemoveUserId(string token);
}