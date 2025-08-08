using game_x.application.Contract.Infrastructure.Security;
using game_x.share.Settings;
using Microsoft.Extensions.Options;

namespace game_x.infrastructure.Security.Encryption;

public sealed class GameAesEncryptor(IOptions<GameProviderSettings> options)
    : AesEncryptor(options.Value.AesKey, options.Value.Iv), IGameAesEncryptor
{
}
