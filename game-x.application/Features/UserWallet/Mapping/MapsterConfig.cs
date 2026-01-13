using game_x.application.Features.Accounts.User.Dtos;

namespace game_x.application.Features.UserWallet.Mapping;

public sealed class MapsterConfig : IRegister
{
    public void Register(TypeAdapterConfig cfg)
    {
        cfg.NewConfig<GamePlatformBalance, UserWalletExternalItemDto>()
            .Map(dest => dest.PlatformId, src => src.Platform.PublicId)
            .Map(dest => dest.PlatformName, src => src.Platform.Name)
            .Map(dest => dest.Amount, src => src.AvailableBalance)
            .Map(dest => dest.LockedAmount, src => src.LockedBalance)
            .Map(dest => dest.TotalAmount, src => src.GetBalance().TotalBalance)
            .Map(dest => dest.LastSyncAt, src => src.LastSyncedAt);
    }
}
