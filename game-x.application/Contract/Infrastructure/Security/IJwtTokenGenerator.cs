namespace game_x.application.Contract.Infrastructure.Security;

public interface IJwtTokenGenerator
{
    Task<JwtTokenDto> GenerateToken(User user);
}

public class JwtTokenDto
{
    public string Token { get; set; } = default!;
    public DateTime ExpiresAt { get; set; }
    public string JwtId { get; set; } = string.Empty;
}
