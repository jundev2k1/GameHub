using game_x.application.Common.Abstractions;
using game_x.application.Contract.Infrastructure.Security;
using game_x.share.Settings;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace game_x.infrastructure.Security;

public sealed class TokenService(IOptions<RefreshTokenSettings> settings) : ITokenService, IServices
{
    public RefreshTokenGenerateDto GenerateRefreshToken(string userId)
    {
        var rawToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var expiresAt = DateTime.UtcNow.AddMinutes(settings.Value.DurationInMinutes);
        return new RefreshTokenGenerateDto
        {
            Token = rawToken,
            ExpiresAt = expiresAt
        };
    }
}
