namespace game_x.application.Features.BankAccountVerifications.Dtos;

public sealed class BankAccountListItemDto
{
    public Guid Id { get; private set; }
    public string UserId { get; private set; } = string.Empty;
    public string BankName { get; private set; } = string.Empty;
    public string BankCode { get; private set; } = string.Empty;
    public string AccountName { get; private set; } = string.Empty;
    public string AccountNumber { get; private set; } = string.Empty;
    public string CurrencyCode { get; private set; } = string.Empty;
    public string CurrencySymbol { get; private set; } = string.Empty;
    public UserBankAccountStatus Status { get; private set; }

    public DateTime? SubmittedAt { get; private set; }
    public DateTime? DateReviewed { get; private set; }
    public string? ReviewedBy { get; private set; }
}
