using game_x.application.Features.OrderManagement.Dtos;

namespace game_x.application.Features.OrderManagement.Staff.Commands.TraceV2.Buy;

public record CreateBuyOrderV2Command(
    string MemberId,
    string PayerBankAccountName,
    PricingMode PricingMode,
    decimal Amount,
    FiatType FiatType,
    CryptoType CryptoType) : ICommand<CreateOrderV2ResponseDto>;