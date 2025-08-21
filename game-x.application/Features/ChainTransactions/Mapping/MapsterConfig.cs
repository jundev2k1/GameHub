using game_x.application.Contract.Infrastructure.SignalR.Dtos;
using game_x.application.Features.ChainTransactions.Dtos;
using game_x.application.Features.ChainTransactions.Shared.Queries.GetCryptoTokenList;

namespace game_x.application.Features.ChainTransactions.Mapping;

public sealed class MapsterConfig : IRegister
{
    public void Register(TypeAdapterConfig cfg)
    {
        cfg.NewConfig<ChainTransaction, TransactionNotificationDto>()
            .Map(dest => dest.Id, src => src.PublicId)
            .Map(dest => dest.Status, src => src.Status.ToString().ToLower())
            .Map(dest => dest.BalanceAfter, src => src.Ledger!.BalanceAfter)
            .Map(dest => dest.CryptoTokenId, src => src.CryptoToken.PublicId);

        cfg.NewConfig<ChainTransaction, ChainTransactionDto>()
            .Map(dest => dest.Id, src => src.PublicId)
            .Map(dest => dest.CryptoTokenId, src => src.CryptoToken.PublicId)
            .Map(dest => dest.Symbol, src => src.CryptoToken.Symbol)
            .Map(dest => dest.Network, src => src.CryptoToken.Network)
            .Map(dest => dest.BalanceAfter, src => src.Ledger!.BalanceAfter);
        
        cfg.NewConfig<ChainTransaction, ChainTransactionDetailDto>()
            .Map(dest => dest.Id, src => src.PublicId)
            .Map(dest => dest.CryptoTokenId, src => src.CryptoToken.PublicId)
            .Map(dest => dest.Symbol, src => src.CryptoToken.Symbol)
            .Map(dest => dest.Network, src => src.CryptoToken.Network)
            .Map(dest => dest.BalanceAfter, src => src.Ledger!.BalanceAfter);
        
        cfg.NewConfig<ChainTransaction, ClientTransactionDto>()
            .Map(dest => dest.TransactionId, src => src.PublicId)
            .Map(dest => dest.Status, src => src.Status.ToString().ToLower())
            .Map(dest => dest.Type, src => src.Type.ToString().ToLower()) ;
        
        cfg.NewConfig<CryptoToken, CryptoTokenDto>()
            .Map(dest => dest.Id, src => src.PublicId);
    }
}
