using game_x.application.Features.OrderManagement.Dtos;

namespace game_x.application.Features.OrderManagement.Staff.Commands.Trade.Sell;

public record CreateSellOrderCommand(
    string MemberId,
    Guid BankAccountId,
    string FiatType,
    decimal FiatAmount) : ICommand<CreateOrderResponseDto>;
