namespace game_x.application.Features.Kyc.Dtos;

public class ReviewerInfoDto
{
    public string Id { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public DateTime ReviewedAt { get; set; }
}
