namespace game_x.application.Contract.Infrastructure.Caching;

public interface IEmailCacheService
{
    public string? GetCode(string email);
    public void SetCode(string email, string code, TimeSpan? expiresIn = null);
    public void RemoveCode(string email);
}