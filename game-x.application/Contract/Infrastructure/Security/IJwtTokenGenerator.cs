namespace game_x.application.Contract.Infrastructure.Security;

public interface IJwtTokenGenerator
{
    Task<JwtTokenDto> GenerateToken(User user);

    JwtPayloadDto DecodeToken(string token);
}

public class JwtTokenDto
{
    public string Token { get; set; } = default!;
    public DateTime ExpiresAt { get; set; }
    public string JwtId { get; set; } = string.Empty;
}

public sealed class JwtPayloadDto
{
    public string? JwtId { get; set; }
    public string? Subject { get; set; }
    public string? Issuer { get; set; }
    public string? Audience { get; set; }
    public DateTime IssuedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsExpired { get; set; }
    public IDictionary<string, object> Claims { get; set; } = new Dictionary<string, object>();
}