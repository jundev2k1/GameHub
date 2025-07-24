namespace game_x.domain.Entities;

public class UserPassport : BaseEntity<int>, IAuditable
{
    public string AppUserId { get; set; } = default!;
    public AppUser AppUser { get; set; } = default!;
    public PassportType PassportType { get; set; } = PassportType.Ordinary;
    public string PassportNumber { get; set; } = default!;
    public string Country { get; set; } = default!;
    public string? IssuedBy { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Gender { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Nationality { get; set; }
    public string? PlaceOfBirth { get; set; }
    public DateTime? IssueDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string? Remarks { get; set; }

    public int? PassportImageId { get; set; }
    public MediaFile? PassportImage { get; set; }

    public bool IsVerified { get; set; }
    public string? VerifiedBy { get; set; }
    public DateTime? VerifiedAt { get; set; }
}