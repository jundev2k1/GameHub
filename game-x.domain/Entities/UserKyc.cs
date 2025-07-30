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
            throw new InvalidOperationException("Can only approve KYC in UnderReview status.");

        Status = KycStatus.Approved;
        ReviewedById = adminId;
        RejectionReason = null;
    }

    public void Reject(string adminId, string reason)
    {
        if (Status != KycStatus.UnderReview)
            throw new InvalidOperationException("Can only reject KYC in UnderReview status.");

        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Rejection reason is required.");

        Status = KycStatus.Rejected;
        ReviewedById = adminId;
        RejectionReason = reason;
    }

    public void Resubmit(string fullName, DateTime dob, string address, string idNumber)
    {
        if (Status != KycStatus.Rejected)
            throw new InvalidOperationException("Only rejected KYC can be resubmitted.");

        FullName = fullName;
        DateOfBirth = dob.ToUniversalTime();
        ResidentialAddress = address;
        IdNumber = idNumber;
        Status = KycStatus.UnderReview;
        SubmittedAt = DateTime.UtcNow;
        DateReviewed = null;
        ReviewedById = null;
        RejectionReason = null;
    }
}
