using game_x.application.Features.OrderManagement.Dtos;

namespace game_x.application.Features.OrderManagement.Staff.Commands.Trade.Buy;

public record CreateBuyOrderCommand(
    string MerchantNumber,
    string MemberId,
    string PayerBankAccountName,
    decimal FiatAmount,
    string? FiatType = CurrencyCode.CNY) : ICommand<CreateOrderResponseDto>;
