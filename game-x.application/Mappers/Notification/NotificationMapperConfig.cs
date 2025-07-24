using game_x.application.Contract.Infrastructure.SignalR.Dtos;
using game_x.application.Features.OrderManagement.Dtos;
using game_x.share.ExternalApi.Uxm.Dtos;
using NotificationEntity = game_x.domain.Entities.Notification;

namespace game_x.application.Mappers.Notification;

public sealed class NotificationMapperConfig : IRegister
{
    public void Register(TypeAdapterConfig cfg)
    {
        cfg.NewConfig<NotificationEntity, NotificationDto>()
            .Map(dest => dest.NotificationId, src => src.PublicId)
            .Map(dest => dest.IsBroadcast, src => src.UserId == null)
            .Map(dest => dest.MessageKey, src => src.MessageKey.ToString())
            .Map(dest => dest.Type, src => src.Type.ToString())
            .Map(dest => dest.Severity, src => src.Severity.ToString());

        cfg.NewConfig<CreateOrderBuyResponseData, CreateOrderResponseDto>().Ignore(dest => dest.IsValid);
    }
}