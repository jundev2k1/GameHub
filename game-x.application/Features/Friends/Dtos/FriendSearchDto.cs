namespace game_x.application.Features.Friends.Dtos;

public sealed record FriendSearchDto
{
    public string UserId { get; set; } = string.Empty;
    public string Nickname { get; set; } = string.Empty;
    public string? Email { get; set; }
    public MediaFile? Avatar { get; set; }
    public Guid? LinkId { get; set; }
    public SocialLinkState? State { get; set; }
    public string? RequesterUserId { get; set; }
    public string? AddresseeUserId { get; set; }
    public DateTime? LinkCreatedAt { get; set; }
}