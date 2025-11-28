using game_x.application.Features.AppSettings.DTOs;

namespace game_x.application.Features.AppSettings.Mapping;

public sealed class MapsterConfig : IRegister
{
    public void Register(TypeAdapterConfig cfg)
    {
        cfg.NewConfig<AppSetting, AppSettingDto>()
            .Map(dest => dest.CanEdit, src => src.IsEditable);
    }
}
