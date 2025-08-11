using game_x.application.Contract.Infrastructure.SignalR.Dtos;
using game_x.application.Features.ChainTransactions.Shared.Queries.GetCryptoTokenList;

namespace game_x.application.Features.ChainTransactions.Mapping;

public sealed class MapsterConfig : IRegister
{
    public void Register(TypeAdapterConfig cfg)
    {
        cfg.NewConfig<ChainTransaction, TransactionNotificationDto>()
            .MapWith(src => new TransactionNotificationDto
            {
                Id = src.PublicId,
                UserId = src.UserId,
                OrderNumber = src.OrderNumber,
                Hash = src.Hash,
                FromAddress = src.FromAddress,
                ToAddress = src.ToAddress,
                Amount = src.Amount,
                Fee = src.Fee,
                ConfirmedAt = src.ConfirmedAt,
                Status = src.Status.ToString(),
                Note = src.Note,
                CreatedAt = src.CreatedAt,
                UpdatedAt = src.UpdatedAt,
                Meta = src.Meta,
                CryptoTokenId = src.CryptoToken.PublicId
            });
        
        cfg.NewConfig<CryptoToken, CryptoTokenDto>()
            .Map(dest => dest.Id, src => src.PublicId);
    }
}
