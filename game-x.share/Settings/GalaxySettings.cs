using System.ComponentModel.DataAnnotations;

namespace game_x.share.Settings;

public sealed class GalaxySettings : BaseSettings
{
    [Required]
    public string MerchantNumber { get; set; } = string.Empty;
}
