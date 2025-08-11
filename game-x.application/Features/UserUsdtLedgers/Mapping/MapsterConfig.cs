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
            .Map(dest => dest.ChainTransactionId, src => src.ChainTransaction != null ? src.ChainTransaction.PublicId : Guid.Empty);
    }
}
