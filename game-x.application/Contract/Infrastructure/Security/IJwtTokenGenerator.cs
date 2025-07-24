using game_x.domain.Identity;

namespace game_x.application.Contract.Infrastructure.Security;

public interface IJwtTokenGenerator
{
    Task<JwtTokenDto> GenerateToken(AppUser user);
}

public class JwtTokenDto
{
    public string Token { get; set; } = default!;
    public DateTime ExpiresAt { get; set; }
}
