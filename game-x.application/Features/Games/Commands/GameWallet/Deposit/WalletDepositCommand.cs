using System.Text.Json.Serialization;

namespace game_x.application.Features.Games.Commands.GameWallet.Deposit;

public record WalletDepositCommand(
    decimal Quota,
    [property: JsonIgnore] string? IpAddress) : ICommand<WalletDepositResult>;

public record WalletDepositResult(
    bool issuccess,
    string message
 );
