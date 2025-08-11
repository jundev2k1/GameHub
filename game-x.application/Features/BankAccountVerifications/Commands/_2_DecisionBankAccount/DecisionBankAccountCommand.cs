namespace game_x.application.Features.BankAccountVerifications.Commands._2_DecisionBankAccount;

public record DecisionBankAccountCommand(
    string UserId,
    KycStatus Status,
    string? Reason,
    string? Details) : ICommand;
