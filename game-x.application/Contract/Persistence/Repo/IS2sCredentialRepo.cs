namespace game_x.application.Contract.Persistence.Repo;

public interface IS2sCredentialRepo
{
    Task<S2SCredential[]> GetByKeyIdAsync(string keyId, CancellationToken ct = default);
    Task<S2SCredential> GetByKeyIdAsync(string keyId, CredentialDirection direction, CancellationToken ct = default);

    Task<S2SCredential[]> GetsBySettingAsync(int settingId, CancellationToken ct = default);

    Task<S2SCredential?> GetActiveCredentialAsync(string appCode, CredentialDirection direction, CancellationToken ct = default);

    Task<S2SCredential?> GetsByKeyIdAsync(string keyId, CancellationToken ct = default);

    Task<bool> CanAddKeyAsync(string appCode, CredentialDirection direction, CancellationToken ct = default);

    Task CreateAsync(S2SCredential entity, CancellationToken ct = default);

    Task CreateRangeAsync(IEnumerable<S2SCredential> entities, CancellationToken ct = default);

    Task UpdateAsync(string keyId, Func<S2SCredential, Task> updateAction, CancellationToken ct = default);

    Task RotateAsync(string keyId, S2SCredential credential, CancellationToken ct = default);
}
