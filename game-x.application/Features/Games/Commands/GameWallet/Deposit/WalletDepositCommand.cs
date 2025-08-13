using System.Text.Json.Serialization;
using game_x.application.Features.Games.Dtos;
using game_x.share.ExternalApi.GameProvider.Dtos.Deposit;

namespace game_x.application.Features.Games.Commands.GameWallet.Deposit;

public record WalletDepositCommand(
    decimal Quota,
    [property: JsonIgnore] string? IpAddress) : ICommand<GameTransactionResponse>;


