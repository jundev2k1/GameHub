using game_x.application.Features.S2s.DTOs;

namespace game_x.application.Contract.Infrastructure.Caching;

public interface IS2sAuthCacheService
{
    Task<S2sCredentialDto> GetAsync(string keyId, AuthMethod method);
}
