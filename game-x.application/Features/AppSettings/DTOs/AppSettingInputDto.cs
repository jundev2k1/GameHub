namespace game_x.application.Features.AppSettings.DTOs;

public sealed class AppSettingInputDto
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}