namespace game_x.application.Features.Friends.Dtos;

public sealed record ListedFriendDto
{
    public string UserId { get; set; } = string.Empty;
    public string Nickname { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public Guid LinkId { get; set; }
    public DateTime? RespondedAt { get; set; }
}