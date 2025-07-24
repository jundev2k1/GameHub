using game_x.application.Features.AuditLogs.Dtos;
using AuditLogEntity = game_x.domain.Entities.AuditLog;

namespace game_x.application.Features.AuditLogs.Mapping;

public sealed class MapsterConfig : IRegister
{
    public void Register(TypeAdapterConfig cfg)
    {
        cfg.NewConfig<AuditLogEntity, AuditLogDto>()
            .Map(dest => dest.Id, src => src.PublicId)
            .Map(dest => dest.EntityName, src => src.EntityName.Value)
            .Map(dest => dest.Action, src => src.Action.ToString())
            .Map(dest => dest.ChangedByUserName, src => src.ChangedBy != null ? src.ChangedBy.UserName : string.Empty)
            .Map(dest => dest.Source, src => src.Source.Value);
    }
}
