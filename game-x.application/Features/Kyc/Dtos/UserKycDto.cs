namespace game_x.application.Features.Kyc.Dtos;

public sealed class UserKycDto
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string ResidentialAddress { get; set; } = string.Empty;
    public string IdNumber { get; set; } = string.Empty;
    public KycType Type { get; set; }
    public string FrontImageName { get; set; } = string.Empty;
    public string FrontImageUrl { get; set; } = string.Empty;
    public string BackImageName { get; set; } = string.Empty;
    public string BackImageUrl { get; set; } = string.Empty;
    public KycStatus Status { get; set; }
    public string? RejectionReason { get; set; }
    public string? RejectDetails { get; set; }
    public DateTime SubmittedAt { get; set; }
    public string? ReviewedById { get; set; }
    public string? ReviewedBy { get; set; }
    public DateTime? DateReviewed { get; set; }
};
