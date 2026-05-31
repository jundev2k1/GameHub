namespace game_x.share.ExternalApi.SasSlot.Dtos.DeletePublicKey;

public sealed class SasSlotDeletePublicKeyRequest
{
    public string PlatformCode { get; set; } = string.Empty;
    public string KeyId { get; set; } = string.Empty;
    public int Ts { get; set; }
    public string Nonce { get; set; } = string.Empty;
}
