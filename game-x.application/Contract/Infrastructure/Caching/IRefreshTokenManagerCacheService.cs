using game_x.application.Features.Auth.Dtos;

namespace game_x.application.Contract.Infrastructure.Caching;

public interface IRefreshTokenManagerCacheService
{
    void InitRefreshTokens();

    IEnumerable<RefreshTokenDto> GetAllTokens();

    IEnumerable<RefreshTokenDto> GetsByUserId(string userId);

    RefreshTokenDto GetToken(string userId, string rawToken);

    RefreshTokenDto? GetTokenByJwtId(string userId, string jwtId);

    void InsertNewToken(RefreshTokenDto tokenDto);

    void ReplaceToken(string userId, string oldTokenHash, string newTokenHash);

    void RevokeToken(string userId, string tokenHash);
    void RevokeToken(string userId, Guid id);
    void RevokeToken(RefreshTokenDto token);

    void UpdateAfterSync(Guid[] tokens);

    void RemoveExpiredTokens();
}
