using game_x.application.Features.OrderManagement.Dtos;

namespace game_x.application.Features.OrderManagement.Staff.Commands.TraceV2.EstimateQuote;
    
public record EstimateQuoteCommand(
    string OrderType,
    decimal Amount,
    PricingMode? PricingMode = PricingMode.FiatAmountFixed,
    FiatType? FiatType = FiatType.Cny,
    CryptoType? CryptoType = CryptoType.Trc20Usdt) : ICommand<EstimateQuoteResponseDto>;