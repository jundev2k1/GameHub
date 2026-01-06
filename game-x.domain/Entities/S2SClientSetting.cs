namespace game_x.domain.Entities;

public sealed class S2SClientSetting : BaseEntity<int>
{
    public string ClientId { get; private set; } = string.Empty;
    public S2SClient Client { get; private set; } = default!;

    public string AppCode { get; private set; } = string.Empty;
    public string AppName { get; private set; } = string.Empty;

    public bool IsActive { get; private set; }

    public string Host { get; private set; } = string.Empty;
    public AllowedIp AllowedIps { get; private set; } = default!;

    public string Notes { get; private set; } = string.Empty;

    public ICollection<S2SCredential> Credentials { get; private set; } = [];

    public static S2SClientSetting Create(
        string clientId,
        string appCode,
        string appName,
        string host,
        AllowedIp allowedIps,
        string notes)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(clientId);
        ArgumentException.ThrowIfNullOrWhiteSpace(appCode);

        return new S2SClientSetting
        {
            ClientId = clientId,
            AppCode = appCode,
            AppName = appName,
            Host = host,
            AllowedIps = allowedIps,
            IsActive = true,
            Notes = notes
        };
    }

    public void UpdateInfo(string appName, string notes)
    {
        AppName = appName.Trim();
        Notes = notes.Trim();
    }

    public void UpdateConfig(string host, AllowedIp allowIps)
    {
        Host = host;
        AllowedIps = allowIps;
    }

    public void UpdateStatus(bool isActive) => IsActive = isActive;
}
