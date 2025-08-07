using game_x.application.Features.ChainTransactions.Dtos;

namespace game_x.application.Features.ChainTransactions.Mapping;

public sealed class MapsterConfig : IRegister
{
    public void Register(TypeAdapterConfig cfg)
    {
        cfg.NewConfig<ChainTransaction, ChainTransactionDto>()
            .Map(dest => dest.Id, src => src.PublicId)
            .Map(dest => dest.CryptoTokenId, src => src.CryptoToken.PublicId);
    }
}
