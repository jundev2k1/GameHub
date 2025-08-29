using game_x.application.Features.Kyc.Dtos;

namespace game_x.application.Features.Kyc.Mapping;

public sealed class MapsterConfig : IRegister
{
    public void Register(TypeAdapterConfig cfg)
    {
        cfg.NewConfig<UserKyc, UserKycDto>()
            .Map(dest => dest.Id, src => src.PublicId)
            .Map(dest => dest.Type, src => src.KycType)
            .Map(dest => dest.ReviewedBy, src => src.ReviewedBy != null ? src.ReviewedBy.UserName : null)
            .Map(dest => dest.FrontImageName, src => src.FrontImage != null ? src.FrontImage.FileName : string.Empty)
            .Map(dest => dest.BackImageName, src => src.BackImage != null ? src.BackImage.FileName : string.Empty);

        cfg.NewConfig<UserKyc, UserKycListItemDto>()
            .Map(dest => dest.Id, src => src.PublicId)
            .Map(dest => dest.Type, src => src.KycType)
            .Map(dest => dest.Email, src => src.User.Email)
            .Map(dest => dest.ReviewedBy, src => src.ReviewedBy != null ? src.ReviewedBy.UserName : null);
    }
}
