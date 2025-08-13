using game_x.application.Features.Auth.Dtos;

namespace game_x.application.Contract.Infrastructure.Caching;

public interface IRefreshTokenManagerCacheService
{
    RefreshTokenDto[] GetAllTokens();

    void InsertNewToken(RefreshTokenDto tokenDto);
}
