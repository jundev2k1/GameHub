using game_x.application.Features.SystemWallets.DTOs;

namespace game_x.application.Features.SystemWallets.Mapping;

public sealed class MapsterConfig : IRegister
{
    public void Register(TypeAdapterConfig cfg)
    {
        cfg.NewConfig<SystemWallet, SystemWalletDto>()
            .Map(dest => dest.Type, src => src.Type)
            .Map(dest => dest.Balance, src => src.Balance);

        cfg.NewConfig<SystemWalletTransaction, SystemWalletDto>()
            .Map(dest => dest.Type, src => src.Wallet.Type);

    }
}
