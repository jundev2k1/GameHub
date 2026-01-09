using System.Security.Cryptography;

namespace game_x.domain.Entities;

public sealed class S2SClient : BaseEntity<string>
{
    public string ClientName { get; private set; } = string.Empty;
    public string ClientCode { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public string Notes { get; private set; } = string.Empty;

    public ICollection<S2SClientSetting> Settings { get; private set; } = [];

    public static S2SClient Create(string clientName, string clientCode, string notes)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(clientName);
        return new S2SClient
        {
            Id = GenerateClientId(),
            ClientName = clientName,
            ClientCode = clientCode,
            IsActive = true,
            Notes = notes
        };
    }

    public void UpdateInfo(string clientName, string clientCode, string notes)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(clientName);

        ClientName = clientName;
        ClientCode = clientCode;
        Notes = notes;
    }

    public void UpdateStatus(bool status) => IsActive = status;

    public void AddSetting(S2SClientSetting setting)
    {
        Settings.Add(setting);
    }

    private static string GenerateClientId()
    {
        // 16 bytes = 128-bit, convert to URL-safe Base32
        var bytes = new byte[16];
        RandomNumberGenerator.Fill(bytes);

        // Base32 (no padding), uppercase for readability
        return Base32Encoding.ToString(bytes).TrimEnd('=').ToUpperInvariant();
    }
}
