using game_x.application.Features.AuditLogManagement.Dtos;
using AuditLogEntity = game_x.domain.Entities.AuditLog;

namespace game_x.application.Mappers.BankAccount;

public sealed class AuditLogMapperConfig : IRegister
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
