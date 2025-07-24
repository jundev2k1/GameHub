using game_x.application.Features.OrderManagement.Dtos;

namespace game_x.application.Features.OrderManagement.Staff.Commands.TraceV2.Sell;

public record CreateSellOrderV2Command(
    string MemberId,
    string PayeeBankName,
    string PayeeBranchCode,
    string PayeeName,
    string PayeeAccountNumber,
    PricingMode PricingMode,
    decimal Amount,
    FiatType FiatType,
    CryptoType CryptoType) : ICommand<CreateOrderV2ResponseDto>;