namespace game_x.application.Features.Kyc.Queries.GetKycProfile;

public record GetKycProfileQuery : IQuery<GetKycProfileResult>;

public record GetKycProfileResult(
    Guid Id,
    string FullName,
    DateTime DateOfBirth,
    string ResidentialAddress,
    string IdNumber,
    string FrontImageName,
    string FrontImageUrl,
    string BackImageName,
    string BackImageUrl,
    KycStatus Status,
    string StatusInfo,
    string? RejectionReason,
    string? RejectDetails,
    DateTime SubmittedAt,
    string? ReviewedById,
    string? ReviewedBy,
    DateTime? DateReviewed);
