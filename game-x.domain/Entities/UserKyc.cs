namespace game_x.domain.Entities;

public sealed class UserKyc : BaseEntity<int>, IAuditable
{
    public Guid PublicId { get; private set; } = Guid.NewGuid();

    public string UserId { get; private set; } = string.Empty;
    public User User { get; private set; } = default!;

    public string FullName { get; private set; } = string.Empty;
    public DateTime DateOfBirth { get; private set; }
    public string ResidentialAddress { get; private set; } = string.Empty;
    public string IdNumber { get; private set; } = string.Empty;

    public int? FrontImageId { get; private set; }
    public MediaFile? FrontImage { get; private set; } = default!;
    public int? BackImageId { get; private set; }
    public MediaFile? BackImage { get; private set; } = default!;

    public KycStatus Status { get; private set; }
    public string? RejectionReason { get; private set; }

    public DateTime? SubmittedAt { get; private set; }
    public DateTime? DateReviewed { get; private set; }
    public string? ReviewedById { get; private set; }
    public User? ReviewedBy { get; private set; }
    public string? RejectDetails { get; private set; }

    public static UserKyc Create(
        string userId,
        string fullName,
        DateTime dateOfBirth,
        string address,
        string idNumber)
    {
        return new UserKyc
        {
            UserId = userId,
            FullName = fullName,
            DateOfBirth = dateOfBirth.ToUniversalTime(),
            ResidentialAddress = address,
            IdNumber = idNumber,
            Status = KycStatus.NotSubmitted,
        };
    }

    public void UploadFrontImage(MediaFile file) => FrontImage = file;

    public void UploadBackImage(MediaFile file) => BackImage = file;

    public void Submit()
    {
        Status = KycStatus.UnderReview;
        SubmittedAt = DateTime.UtcNow;
        RejectionReason = null;
    }

    public void Approve(string adminId)
    {
        if (Status != KycStatus.UnderReview)
            throw new ArgumentException("Can only approve KYC in UnderReview status.");

        Status = KycStatus.Approved;
        ReviewedById = adminId;
        RejectionReason = null;
        DateReviewed = DateTime.UtcNow;
    }

    public void Reject(string adminId, string? reason, string? details = "")
    {
        if (Status != KycStatus.UnderReview)
            throw new ArgumentException("Can only reject KYC in UnderReview status.");

        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Rejection reason is required.");

        if (details.IsNotNullOrEmpty() && !JsonHelper.IsJsonArray(details!))
            throw new ArgumentException("Reject details must be an array object json.");

        Status = KycStatus.Rejected;
        ReviewedById = adminId;
        RejectionReason = reason;
        RejectDetails = details;
        DateReviewed = DateTime.UtcNow;
    }

    public void Resubmit(string? fullName, DateTime? dob, string? address, string? idNumber)
    {
        if (Status != KycStatus.Rejected && Status != KycStatus.UnderReview)
            throw new ArgumentException("Only rejected KYC can be resubmitted.");

        FullName = fullName ?? FullName;
        DateOfBirth = dob?.ToUniversalTime() ?? DateOfBirth;
        ResidentialAddress = address ?? ResidentialAddress;
        IdNumber = idNumber ?? IdNumber;
        Status = KycStatus.UnderReview;
        SubmittedAt = DateTime.UtcNow;
        DateReviewed = null;
        ReviewedById = null;
        RejectionReason = null;
        RejectDetails = null;
    }
}
