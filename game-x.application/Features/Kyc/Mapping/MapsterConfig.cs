using game_x.application.Features.Kyc.Dtos;
using game_x.application.Features.Kyc.Queries.GetKycStatus;

namespace game_x.application.Features.Kyc.Mapping;

public sealed class MapsterConfig : IRegister
{
    public void Register(TypeAdapterConfig cfg)
    {
        cfg.NewConfig<UserKyc, GetKycStatusResult>()
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.RejectionReason, src => src.RejectionReason)
            .Map(dest => dest.SubmittedAt, src => src.SubmittedAt)
            .Map(dest => dest.ReviewedBy, src => src.ReviewedBy == null
                ? null
                : new ReviewerInfoDto
                {
                    Id = src.ReviewedById ?? string.Empty,
                    Username = src.ReviewedBy.UserName ?? string.Empty,
                    ReviewedAt = src.DateReviewed ?? DateTime.UtcNow,
                });
    }
}
