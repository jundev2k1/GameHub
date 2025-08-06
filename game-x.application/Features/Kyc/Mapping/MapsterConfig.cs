using game_x.application.Features.Kyc.Dtos;
using game_x.application.Features.Kyc.Queries.GetKycProfile;
using game_x.application.Features.Kyc.Queries.GetKycStatus;

namespace game_x.application.Features.Kyc.Mapping;

public sealed class MapsterConfig : IRegister
{
    public void Register(TypeAdapterConfig cfg)
    {
        cfg.NewConfig<UserKyc, GetKycProfileResult>()
            .Map(dest => dest.Id, src => src.PublicId)
            .Map(dest => dest.StatusInfo, src => src.Status.ToString())
            .Map(dest => dest.ReviewedBy, src => src.ReviewedBy != null ? src.ReviewedBy.UserName : null)
            .Map(dest => dest.FrontImageName, src => src.FrontImage != null ? src.FrontImage.FileName : string.Empty)
            .Map(dest => dest.BackImageName, src => src.BackImage != null ? src.BackImage.FileName : string.Empty);
    }
}
