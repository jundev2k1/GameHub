using game_x.application.Features.Games.Dtos;
using System.Text.Json.Serialization;

namespace game_x.application.Features.Games.Client.Commands.GameWallet.Deposit;

public record WalletDepositCommand(
    [property: JsonIgnore] Guid PlatformId,
    decimal Amount,
    Guid CryptoTokenId,
    string? Note) : ICommand<GameTransactionDto>;