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

        cfg.NewConfig<S2SCredentialMaterial, S2sCredentialMaterialDto>()
            .Map(dest => dest.Type, src => src.Type)
            .Map(dest => dest.Value, src => src.Value)
            .Map(dest => dest.IsEncrypted, src => src.IsEncrypted);

        cfg.NewConfig<S2SCredential, S2sCredentialDto>()
            .Map(dest => dest.Materials, src => src.Materials.Adapt<S2sCredentialMaterialDto[]>());

        cfg.NewConfig<S2SClientSetting, S2sClientSettingDetailDto>()
            .Map(dest => dest.ClientName, src => src.Client.ClientName)
            .Map(dest => dest.ClientCode, src => src.Client.ClientCode)
            .Map(dest => dest.IsClientActive, src => src.Client.IsActive)
            .Map(dest => dest.ClientNotes, src => src.Client.Notes)
            .Map(dest => dest.IsSettingActive, src => src.IsActive)
            .Map(dest => dest.SettingNotes, src => src.Notes)
            .Map(dest => dest.AllowedIpsString, src => src.AllowedIps)
            .Map(dest => dest.Credentials, src => src.Credentials.Adapt<S2sCredentialDto[]>())
            .Map(dest => dest.ClientCreatedAt, src => src.Client.CreatedAt)
            .Map(dest => dest.ClientUpdatedAt, src => src.Client.UpdatedAt)
            .Map(dest => dest.SettingCreatedAt, src => src.CreatedAt)
            .Map(dest => dest.SettingUpdatedAt, src => src.UpdatedAt);
    }
}
