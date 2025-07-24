using game_x.application.Features.AccountManagement.Dtos;
using game_x.domain.Identity;

namespace game_x.application.Mappers.User;

public sealed class UserMapperConfig : IRegister
{
    public void Register(TypeAdapterConfig cfg)
    {
        cfg.NewConfig<UserMappingDto, UserDto>()
            .Map(dest => dest.PassportNumber, src => src.Passport != null ? src.Passport.PassportNumber : null)
            .Map(dest => dest.CreatedById, src => src.Staff != null ? src.Staff.Id : null)
            .Map(dest => dest.CreatedByName, src => src.Staff != null ? src.Staff.UserName : null);

        cfg.NewConfig<AppUser, UserDetailDto>()
            .Map(dest => dest.Roles, src => src.UserRoles.Select(ur => ur.Role.Name).Where(r => r != null).ToArray())
            .Map(dest => dest.CreatedById, src => src.StaffUser != null ? src.StaffUser.Staff.Id : null)
            .Map(dest => dest.CreatedByName, src => src.StaffUser != null ? src.StaffUser.Staff.UserName : null);
    }
}