using System.Text.Json.Serialization;

namespace game_x.application.Features.BankAccountVerifications.Commands._2_DecisionBankAccount;

public record DecisionBankAccountCommand(
    [property: JsonIgnore] Guid Id,
    UserBankAccountStatus Status,
    string? Reason,
    string? Details) : ICommand;
