namespace game_x.application.Features.BankAccountVerifications.Queries.GetBankAccountProfile;

public record GetBankAccountProfileQuery(string Code) : IQuery<GetBankAccountProfileResult>;

public record GetBankAccountProfileResult(
    Guid Id,
    string BankName,
    string BankCode,
    string AccountName,
    string AccountNumber,
    string CurrencyCode,
    string ImageName,
    string? ImageUrl,
    UserBankAccountStatus Status,
    string? RejectionReason,
    string? RejectDetails,
    DateTime SubmittedAt,
    string? ReviewedById,
    string? ReviewedBy,
    DateTime? DateReviewed);
