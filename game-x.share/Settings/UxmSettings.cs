using System.ComponentModel.DataAnnotations;

namespace game_x.share.Settings;

public sealed class UxmSettings : BaseSettings
{
    [Required]
    public string Host { get; set; } = string.Empty;
    [Required]
    public int TransactionExpireTimeSeconds { get; set; }
}