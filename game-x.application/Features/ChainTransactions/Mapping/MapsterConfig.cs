using game_x.application.Contract.Infrastructure.SignalR.Dtos;
using game_x.application.Features.Accounts.User.Queries.GetSelfUserBalance;
using game_x.application.Features.ChainTransactions.Admin.Queries.GetTransactionCriteriaByAdmin;
using game_x.application.Features.ChainTransactions.Dtos;
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
                BalanceAfter = src.Ledger!.BalanceAfter,
                ConfirmedAt = src.ConfirmedAt,
                Status = src.Status.ToString(),
                Note = src.Note,
                CreatedAt = src.CreatedAt,
                UpdatedAt = src.UpdatedAt,
                Meta = src.Meta,
                CryptoTokenId = src.CryptoToken.PublicId
            });

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
        
        cfg.NewConfig<CryptoToken, CryptoTokenDto>()
            .Map(dest => dest.Id, src => src.PublicId);
    }
}
