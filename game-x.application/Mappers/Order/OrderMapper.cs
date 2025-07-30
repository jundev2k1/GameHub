// using game_x.application.Features.OrderManagement.Client.Commands.Trade.Buy;
// using game_x.application.Features.OrderManagement.Dtos;
// using game_x.share.ExternalApi.Uxm.Dtos;

// namespace game_x.application.Mappers.Order
// {
//     public class OrderMapper
//     {
//         public CreateOrderBuyRequestData ToCreateOrderBuyRequestData(CreateBuyOrderCommand request, string orderUid, string merchantNumber)
//         {
//             return new CreateOrderBuyRequestData
//             {
//                 OrderUid = orderUid,
//                 Amount = request.FiatAmount,
//                 Currency = request.FiatType ?? "USDT",
//                 MerchantNumber = merchantNumber
//             };
//         }

//         public CreateOrderResponseDto ToCreateOrderDto(CreateDepositResponse data, bool isVerified)
//         {
//             return new CreateOrderResponseDto
//             {
//                 OrderUid = data.OrderUid,
//                 Amount = data.Amount,
//                 IsVerified = isVerified
//             };
//         }
//     }
// }
