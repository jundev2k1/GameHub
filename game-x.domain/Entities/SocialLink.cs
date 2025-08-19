using System.ComponentModel.DataAnnotations;

namespace game_x.domain.Entities;

/// <summary>
///     Gate sending in DMs; drive “ensure or open DM conversation” on acceptance
/// </summary>
public sealed class SocialLink: BaseEntity<int>, IAuditable
{
    public Guid PublicId { get; set; }
    [MaxLength(64)] 
    public string UserIdMin { get; set; } = null!;
    [MaxLength(64)] 
    public string UserIdMax { get; set; } = null!;

    public SocialLinkKind Kind { get; set; }
    public SocialLinkState State { get; set; }

    // Friendship fields
    [MaxLength(64)] 
    public string? RequesterUserId { get; set; }
    public User? RequesterUser { get; set; }
    [MaxLength(64)] 
    public string? AddresseeUserId { get; set; }
    public User? AddresseeUser { get; set; }

    // Block fields (directional): who blocked whom
    [MaxLength(64)] 
    public string? BlockerUserId { get; set; }
    public User? BlockerUser { get; set; }
    [MaxLength(64)] 
    public string? BlockedUserId { get; set; }
    public User? BlockedUser { get; set; }
    
    public DateTime? RespondedAt { get; set; }
}