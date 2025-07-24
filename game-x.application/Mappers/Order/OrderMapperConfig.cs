using game_x.application.Features.OrderManagement.Dtos;
using game_x.application.Features.OrderManagement.Staff.Commands.Trade.Buy;
using game_x.share.ExternalApi.Uxm.Dtos;
using OrderEntity = game_x.domain.Entities.Order;

namespace game_x.application.Mappers.Order;

public sealed class OrderMapperConfig : IRegister
{
    public void Register(TypeAdapterConfig cfg)
    {
        cfg.NewConfig<CreateBuyOrderCommand, CreateOrderBuyRequestData>();

        cfg.NewConfig<CreateOrderBuyResponseData, CreateOrderResponseDto>().Ignore(dest => dest.IsValid);

        cfg.NewConfig<OrderEntity, OrderDto>()
            .Map(dest => dest.OrderId, src => src.PublicId)
            .Map(dest => dest.UxmOrderId, src => src.OrderUid)
            .Map(dest => dest.CounterId, src => src.Counter.PublicId)
            .Map(dest => dest.CounterNumber, src => src.Counter.CounterNumber.Value)
            .Map(dest => dest.OwnerName, src => src.User.UserName)
            .Map(dest => dest.OwnerEmail, src => src.User.Email)
            .Map(dest => dest.StaffName, src => src.Staff.UserName)
            .Map(dest => dest.OrderType, src => src.OrderType.Value)
            .Map(dest => dest.CurrencyUnit, src => src.CurrencyUnit != null ? src.CurrencyUnit.Value : null)
            .Map(dest => dest.OrderStatus, src => src.OrderStatus.Value)
            .Map(dest => dest.UxmFee, src => src.Fee);
        
        cfg.NewConfig<UxmOrderDetailInfoResponse, OrderDetailInfoDto>()
            .Map(dest => dest.UxmOrderStatus, src => src.Status)
            .Map(dest => dest.UxmDisputeStatus, src => src.DisputeStatus)
            .Map(dest => dest.UxmCreatedAt, src => src.CreatedAt)
            .Map(dest => dest.UxmCompletedAt, src => src.CompletedAt); 

        cfg.NewConfig<EstimateQuoteResponse, EstimateQuoteResponseDto>()
            .Map(dest => dest.UxmFee, src => src.Fee)
        .Map(dest => dest.OrderType, src => OrderType.GetOrderType(src.Direction).Value);
    }
}