using game_x.application.Contract.Infrastructure.SignalR.Dtos;
using game_x.application.Features.UserUsdtLedgers.Dtos;

namespace game_x.application.Features.UserUsdtLedgers.Mapping;

public sealed class MapsterConfig : IRegister
{
    public void Register(TypeAdapterConfig cfg)
    {
        cfg.NewConfig<UserUsdtLedger, UserUsdtLedgerNotificationDto>()
            .MapWith(src => new UserUsdtLedgerNotificationDto
            {
                Id = src.PublicId,
                UserId = src.UserId,
                Timestamp = src.Timestamp,
                FlowType = src.FlowType,
                SourceId = src.SourceId,
                ChangeAmount = src.ChangeAmount,
                BalanceAfter = src.BalanceAfter,
                ChainTransactionId = src.ChainTransaction!.PublicId,
                Meta = src.Meta,
                CreatedAt = src.CreatedAt,
                UpdatedAt = src.UpdatedAt,
            });
        
        cfg.NewConfig<UserUsdtLedger, UserUsdtLedgerDto>()
            .Map(dest => dest.Id, src => src.PublicId)
            .Map(dest => dest.ChainTransactionId, src => src.ChainTransaction != null ? src.ChainTransaction.PublicId : Guid.Empty)
            .Map(dest => dest.ChainTransactionStatus, src => src.ChainTransaction!.Status)
            .Map(dest => dest.Network, src => src.ChainTransaction!.CryptoToken.Network)
            .Map(dest => dest.Symbol, src => src.ChainTransaction!.CryptoToken.Symbol);        
        
        cfg.NewConfig<UserUsdtLedger, UserUsdtLedgerDetailDto>()
            .Map(dest => dest.Id, src => src.PublicId)
            .Map(dest => dest.ChainTransactionId, src => src.ChainTransaction != null ? src.ChainTransaction.PublicId : Guid.Empty)
            .Map(dest => dest.Amount, src => src.ChainTransaction != null ? src.ChainTransaction.Amount : 0)
            .Map(dest => dest.Fee, src => src.ChainTransaction != null ? src.ChainTransaction.Fee : 0)
            .Map(dest => dest.ChainTransactionStatus, src => src.ChainTransaction!.Status)
            .Map(dest => dest.Network, src => src.ChainTransaction!.CryptoToken.Network)
            .Map(dest => dest.Symbol, src => src.ChainTransaction!.CryptoToken.Symbol);
    }
}
