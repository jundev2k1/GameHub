using game_x.application.Features.AccountManagement.Dtos;
using game_x.domain.Identity;

namespace game_x.application.Mappers.Staff;

public sealed class StaffMapperConfig : IRegister
{
    public void Register(TypeAdapterConfig cfg)
    {
        cfg.NewConfig<StaffMappingDto, StaffDto>()
            .Map(dest => dest.CreatedById, src => src.Admin != null ? src.Admin.Id : null)
            .Map(dest => dest.CreatedByName, src => src.Admin != null ? src.Admin.UserName : null);

        cfg.NewConfig<AppUser, StaffDetailDto>()
            .Map(dest => dest.Roles, src => src.UserRoles.Select(ur => ur.Role.Name).Where(r => r != null).ToArray())
            .Map(dest => dest.CreatedById,
                src => src.StaffExtension != null && src.StaffExtension.Admin != null
                    ? src.StaffExtension.Admin.Id
                    : null)
            .Map(dest => dest.CreatedByName,
                src => src.StaffExtension != null && src.StaffExtension.Admin != null
                    ? src.StaffExtension.Admin.UserName
                    : null);
    }
}