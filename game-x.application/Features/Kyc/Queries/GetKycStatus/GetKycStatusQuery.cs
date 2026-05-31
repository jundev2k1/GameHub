namespace game_x.application.Features.Kyc.Queries.GetKycStatus;

public record GetKycStatusQuery : IQuery<GetKycStatusResult>;

public record GetKycStatusResult(KycStatus Status, string? RejectionReason = null);
