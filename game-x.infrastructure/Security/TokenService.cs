using game_x.application.Common.Abstractions;
using game_x.application.Contract.Infrastructure.Security;
using System.Security.Cryptography;

namespace game_x.infrastructure.Security;

public sealed class TokenService : ITokenService, IServices
{
    public RefreshTokenGenerateDto GenerateRefreshToken(string userId)
    {
        var rawToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var expiresAt = DateTime.UtcNow.AddDays(7);
        return new RefreshTokenGenerateDto
        {
            Token = rawToken,
            ExpiresAt = expiresAt
        };
    }
}
