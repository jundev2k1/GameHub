using System.ComponentModel.DataAnnotations;

namespace game_x.share.Settings;

public sealed class GameXSettings : BaseSettings
{
    [Required]
    public string MerchantNumber { get; set; } = string.Empty;
    [Required]
    public string PaymentGatewayApiKey { get; set; } = string.Empty;
    [Required]
    public string PaymentGatewaySecretKey { get; set; } = string.Empty;
    public required string AesKey { get; set; } = string.Empty;
    public string PublicWebUrl { get; set; } = string.Empty;
}