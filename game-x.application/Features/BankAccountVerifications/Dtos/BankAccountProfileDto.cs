namespace game_x.application.Features.BankAccountVerifications.Dtos;

public sealed class BankAccountProfileDto
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;
    public string BankCode { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public string CurrencyCode { get; set; } = string.Empty;
    public string CurrencySymbol { get; set; } = string.Empty;
    public UserBankAccountStatus Status { get; set; }
    public string ImageName { get; set; } = string.Empty;
    public string? ImageUrl { get; set; } = string.Empty;
    public string? RejectionReason { get; set; }
    public string? RejectDetails { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public DateTime? DateReviewed { get; set; }
    public string? ReviewedById { get; set; }
    public string? ReviewedBy { get; set; }
};
