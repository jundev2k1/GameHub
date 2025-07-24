using game_x.application.Contract.Infrastructure.SignalR.Dtos;

namespace game_x.application.Features.Notifications.Mapping;

public sealed class NotificationMapperConfig : IRegister
{
    public void Register(TypeAdapterConfig cfg)
    {
        cfg.NewConfig<Notification, NotificationDto>()
            .Map(dest => dest.NotificationId, src => src.PublicId)
            .Map(dest => dest.IsBroadcast, src => src.UserId == null)
            .Map(dest => dest.MessageKey, src => src.MessageKey.ToString())
            .Map(dest => dest.Type, src => src.Type.ToString())
            .Map(dest => dest.Severity, src => src.Severity.ToString());
    }
}