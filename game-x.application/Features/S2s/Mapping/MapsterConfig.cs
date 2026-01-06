using game_x.application.Features.S2s.DTOs;

namespace game_x.application.Features.S2s.Mapping;

public sealed class MapsterConfig : IRegister
{
    public void Register(TypeAdapterConfig cfg)
    {
        cfg.NewConfig<S2SClientSetting, S2sClientSettingDto>()
            .Map(dest => dest.LocalId, src => src.Id)
            .Map(dest => dest.ClientId, src => src.ClientId)
            .Map(dest => dest.AllowIps, src => src.AllowedIps.GetAllowIps());

        cfg.NewConfig<S2SClient, S2sClientDto>()
            .Map(dest => dest.ClientId, src => src.Id)
            .Map(dest => dest.Settings, src => src.Settings.Select(s => s.Adapt<S2sClientSettingDto>()));
    }
}
