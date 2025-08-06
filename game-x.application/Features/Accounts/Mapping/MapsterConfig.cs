using game_x.application.Features.Accounts.Dtos;
using game_x.application.Features.Accounts.User.Queries.GetSelfUser;
using UserEntity = game_x.domain.Entities.User;

namespace game_x.application.Features.Accounts.Mapping;

public sealed class MapsterConfig : IRegister
{
    public void Register(TypeAdapterConfig cfg)
    {
        cfg.NewConfig<UserEntity, UserDetailDto>()
            .Map(dest => dest.UserId, src => src.Id)
            .Map(dest => dest.Username, src => src.UserName)
            .Map(dest => dest.DateOfBirth, src => src.UserKyc != null ? src.UserKyc.DateOfBirth : (DateTime?)null)
            .Map(dest => dest.ResidentialAddress, src => src.UserKyc != null ? src.UserKyc.ResidentialAddress : string.Empty)
            .Map(dest => dest.DateOfBirth, src => src.UserKyc != null ? src.UserKyc.DateOfBirth : (DateTime?)null)
            .Map(dest => dest.IsKycConfirmed, src => src.UserKyc != null && src.UserKyc.Status == KycStatus.Approved);
        cfg.NewConfig<UserDetailDto, GetSelfUserResult>()
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest.Roles, src => src.Roles.Items);
    }
}
