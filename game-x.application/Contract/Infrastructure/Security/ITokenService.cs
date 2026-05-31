namespace game_x.application.Contract.Infrastructure.Security;

public interface ITokenService
{
    RefreshTokenGenerateDto GenerateRefreshToken(string userId);
}

public sealed class RefreshTokenGenerateDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}
