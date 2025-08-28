namespace game_x.application.Features.Kyc.Dtos;

public sealed class UserKycListItemDto
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public KycType Type { get; set; }
    public KycStatus Status { get; set; }
    public DateTime SubmittedAt { get; set; }
    public string? ReviewedById { get; set; }
    public string? ReviewedBy { get; set; }
    public DateTime? DateReviewed { get; set; }
};
