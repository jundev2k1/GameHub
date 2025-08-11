namespace game_x.domain.Entities;

public sealed class UserBankAccount : BaseEntity<int>, IAuditable
{
    public Guid PublicId { get; private set; }
    public string UserId { get; private set; } = string.Empty;
    public User User { get; private set; } = default!;
    public string BankName { get; private set; } = string.Empty;
    public string BankCode { get; private set; } = string.Empty;
    public string AccountName { get; private set; } = string.Empty;
    public string AccountNumber { get; private set; } = string.Empty;
    public int CurrencyId { get; private set; }
    public FiatCurrency FiatCurrency { get; private set; } = default!;
    public int? ImageId { get; private set; }
    public MediaFile? Image { get; private set; } = default!;
    public UserBankAccountStatus Status { get; private set; }
    public string? RejectionReason { get; private set; }

    public DateTime? SubmittedAt { get; private set; }
    public DateTime? DateReviewed { get; private set; }
    public string? ReviewedById { get; private set; }
    public User? ReviewedBy { get; private set; }
    public string? RejectDetails { get; private set; }

    public static UserBankAccount Create(
        string userId,
        string bankName,
        string bankCode,
        string accountName,
        string accountNumber,
        int currencyId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId, nameof(userId));
        ArgumentException.ThrowIfNullOrWhiteSpace(bankName, nameof(bankName));
        ArgumentException.ThrowIfNullOrWhiteSpace(bankCode, nameof(bankCode));
        ArgumentException.ThrowIfNullOrWhiteSpace(accountName, nameof(accountName));
        ArgumentException.ThrowIfNullOrWhiteSpace(accountNumber, nameof(accountNumber));
        if (currencyId <= 0)
            throw new ArgumentException("Currency ID must be a valid positive integer.", nameof(currencyId));

        return new UserBankAccount()
        {
            UserId = userId,
            BankName = bankName,
            BankCode = bankCode,
            AccountName = accountName,
            AccountNumber = accountNumber,
            CurrencyId = currencyId,
            Status = UserBankAccountStatus.NotSubmitted,
        };
    }

    public void UploadImage(MediaFile file)
        =>  Image = file ?? throw new ArgumentNullException(nameof(file), "Image file cannot be null.");

    public void Submit()
    {
        Status = UserBankAccountStatus.UnderReview;
        SubmittedAt = DateTime.UtcNow;
        RejectionReason = null;
        RejectDetails = null;
    }

    public void ReSubmit()
    {
        if ((Status != UserBankAccountStatus.Rejected) && (Status != UserBankAccountStatus.UnderReview))
            throw new InvalidOperationException("Can only resubmit in Rejected status.");

        Status = UserBankAccountStatus.UnderReview;
        SubmittedAt = DateTime.UtcNow;
        RejectionReason = null;
        RejectDetails = null;
    }

    public void Approve(string reviewedById)
    {
        if (Status != UserBankAccountStatus.UnderReview)
            throw new ArgumentException("Can only approve KYC in UnderReview status.");

        ReviewedById = reviewedById;
        Status = UserBankAccountStatus.Approved;
        DateReviewed = DateTime.UtcNow;
    }

    public void Reject(string reason, string details)
    {
        if (Status != UserBankAccountStatus.UnderReview)
            throw new ArgumentException("Can only approve KYC in UnderReview status.");

        RejectionReason = reason;
        RejectDetails = details;
        Status = UserBankAccountStatus.Rejected;
        DateReviewed = DateTime.UtcNow;
    }
}
