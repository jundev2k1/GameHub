using System.Text.Json.Serialization;
using game_x.share.ExternalApi.GameProvider.Dtos.Withdrawal;

namespace game_x.application.Features.Games.Commands.GameWallet.Withdrawal;

public record WalletWithdrawalCommand(
    decimal Quota,
    [property: JsonIgnore] string? IpAddress) : ICommand<WalletWithdrawalResponse>;


