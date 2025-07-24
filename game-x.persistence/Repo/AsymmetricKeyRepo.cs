using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;

namespace game_x.persistence.Repo;

public sealed class AsymmetricKeyRepo(GameXContext context) : IAsymmetricKeyRepo
{
    public async Task<AsymmetricKey> GetByCompositeKeyAsync(string name, KeyType keyType, string algorithm, CancellationToken ct = default)
    {
        var result = await context.AsymmetricKey
            .AsNoTracking()
            .FirstOrDefaultAsync(x => (x.Name == name) && (x.KeyType == keyType) && (x.Algorithm == algorithm), ct)
            ?? throw new NotFoundException(name, keyType.ToString());
        return result;
    }
}
