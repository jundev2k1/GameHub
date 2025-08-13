namespace game_x.application.Features.Accounts.User.Dtos;

public sealed class VerificationStatusDto
{
    public string? CurrencyCode { get; set; } = string.Empty;
    public VerificationStatusType Type { get; set; }
    public VerificationStatus Status { get; set; }
    public bool IsVerified { get; set; }
    public string? RejectionReason { get; set; }
    public string? RejectDetails { get; set; }
}

public enum VerificationStatusType
{
    Kyc = 0,
    BankAccount = 1,
}

public enum VerificationStatus
{
    NotSubmitted = 0,
    UnderReview = 1,
    Approved = 2,
    Rejected = 3,
}
