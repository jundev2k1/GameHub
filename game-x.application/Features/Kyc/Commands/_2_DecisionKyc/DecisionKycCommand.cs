namespace game_x.application.Features.Kyc.Commands._2_DecisionKyc;

public record DecisionKycCommand(
    string UserId,
    KycStatus Status,
    string? Reason,
    string? Details) : ICommand;
