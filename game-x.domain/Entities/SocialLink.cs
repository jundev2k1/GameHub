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
    
    public static SocialLink Create(
        string min,
        string max,
        SocialLinkState state,
        SocialLinkKind kind,
        string? requesterUserId = null,
        string? addresseeUserId = null,
        string? blockerUserId = null,
        string? blockedUserId = null
    )
    {
        var link = new SocialLink
        {
            UserIdMin = min,
            UserIdMax = max,
            Kind = kind,
            State = state,
            RequesterUserId = requesterUserId,
            AddresseeUserId = addresseeUserId,
            BlockerUserId = blockerUserId,
            BlockedUserId = blockedUserId,
        };
        return link;
    }

    public void Respond(bool accept)
    {
        State = accept ? SocialLinkState.Accepted : SocialLinkState.Declined;
        RespondedAt = DateTime.UtcNow;
    }
}

public static class SocialLinkPair
{
    public static (string Min, string Max) Normalize(string a, string b)
        => string.CompareOrdinal(a, b) < 0 ? (a, b) : (b, a);
}