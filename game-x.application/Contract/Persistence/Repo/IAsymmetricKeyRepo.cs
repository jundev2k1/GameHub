namespace game_x.application.Contract.Persistence.Repo;

public interface IAsymmetricKeyRepo
{
    Task<AsymmetricKey> GetByCompositeKeyAsync(string name, KeyType keyType, string algorithm, CancellationToken ct = default);
}
