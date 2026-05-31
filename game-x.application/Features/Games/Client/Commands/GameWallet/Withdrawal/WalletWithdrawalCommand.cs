using game_x.application.Features.Games.Dtos;
using System.Text.Json.Serialization;

namespace game_x.application.Features.Games.Client.Commands.GameWallet.Withdrawal;

public record WalletWithdrawalCommand(
    [property: JsonIgnore] Guid PlatformId,
    Guid CryptoTokenId,
    string? Note) : ICommand<ListTransactionExternalDto>;
