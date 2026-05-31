namespace game_x.application.Contract.Infrastructure.Caching;

public interface IEmailCacheService
{
    public string? GetCode(string email, string purpose);

    public void SetCode(string email, string purpose, string code, TimeSpan? expiresIn = null);

    public void RemoveCode(string email, string purpose);
}
