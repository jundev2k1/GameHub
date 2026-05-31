namespace game_x.share.ExternalApi.SasSlot.Dtos.RegisterPublicKey;

public sealed class SasSlotRegisterPublicKeyRequest
{
    public string PlatformCode { get; set; } = string.Empty;
    public string KeyId { get; set; } = string.Empty;
    public string PublicKeyPem { get; set; } = string.Empty;
    public int Ts { get; set; }
    public string Nonce { get; set; } = string.Empty;
}
