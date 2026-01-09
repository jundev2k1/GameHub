namespace game_x.application.Features.S2s.DTOs;

public sealed class S2sClientDto
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public string ClientCode { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string Notes { get; set; } = string.Empty;
    public S2sClientSettingDto[] Settings { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
