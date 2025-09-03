namespace game_x.share.ExternalApi.GameProvider.Dtos.Wallet;

public sealed class GameWalletResponse : ResponseBase
{
    public GameWalletInfoDto Data { get; set; } = default!;
}
