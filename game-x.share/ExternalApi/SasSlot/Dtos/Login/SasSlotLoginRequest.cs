using game_x.share.ExternalApi.SasSlot.Constants;

namespace game_x.share.ExternalApi.SasSlot.Dtos.Login;

public sealed class SasSlotLoginRequest
{
    public string PlatformCode { get; set; } = string.Empty;
    public string ExtUserId { get; set; } = string.Empty;
    public string Nickname { get; set; } = string.Empty;
    public int Ts { get; set; }
    public string Nonce { get; set; } = string.Empty;
    public string Lang { get; set; } = SasSlotLanguage.TW;
}
