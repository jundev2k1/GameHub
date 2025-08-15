using game_x.application.Features.Auth.Dtos;

namespace game_x.application.Contract.Infrastructure.Caching;

public interface IRefreshTokenManagerCacheService
{
    RefreshTokenDto[] GetAllTokens();

    RefreshTokenDto GetToken(string rawToken);

    void InsertNewToken(RefreshTokenDto tokenDto);

    void ReplaceToken(string oldTokenHash, string newTokenHash);

    void RevokeToken(string tokenHash);
}
