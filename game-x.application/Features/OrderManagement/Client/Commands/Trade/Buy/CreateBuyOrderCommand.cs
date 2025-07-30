using game_x.application.Features.OrderManagement.Dtos;

namespace game_x.application.Features.OrderManagement.Client.Commands.Trade.Buy;

public record CreateBuyOrderCommand(
    string MerchantNumber,
    decimal amount,
    string orderNumber,
    string userId,
    string remark) : ICommand<CreateOrderResponseDto>;
