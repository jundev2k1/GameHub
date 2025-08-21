using game_x.application.Features.Games.Dtos;

namespace game_x.application.Features.Games.Mapping;

public sealed class MapsterConfig : IRegister
{
    public void Register(TypeAdapterConfig cfg)
    {
        cfg.NewConfig<GameTransaction, GameTransactionDto>()
            .Map(dest => dest.Id, src => src.PublicId)
            .Map(dest => dest.CryptoTokenId, src => src.CryptoToken!.PublicId)
            .Map(dest => dest.Symbol, src => src.CryptoToken!.Symbol)
            .Map(dest => dest.Network, src => src.CryptoToken!.Network)
            .Map(dest => dest.BalanceAfter, src => src.Ledger!.BalanceAfter);
        
        cfg.NewConfig<GameTransaction, GameTransactionDetailDto>()
            .Map(dest => dest.Id, src => src.PublicId)
            .Map(dest => dest.CryptoTokenId, src => src.CryptoToken!.PublicId)
            .Map(dest => dest.Symbol, src => src.CryptoToken!.Symbol)
            .Map(dest => dest.Network, src => src.CryptoToken!.Network)
            .Map(dest => dest.BalanceAfter, src => src.Ledger!.BalanceAfter);
        
    }
}
