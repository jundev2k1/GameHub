namespace game_x.application.Features.AppSettings.DTOs;

public sealed class AppSettingDto
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool CanEdit { get; set; }
}
