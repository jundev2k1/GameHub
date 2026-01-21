using game_x.application.Contract.Infrastructure.SignalR.Dtos.Transactions;
using game_x.application.Features.Transactions.Dtos;
using game_x.application.Features.Transactions.Shared.Queries.GetCryptoTokenList;
using game_x.share.Extensions;

namespace game_x.application.Features.Transactions.Mapping;

public sealed class MapsterConfig : IRegister
{
    public void Register(TypeAdapterConfig cfg)
    {
        cfg.NewConfig<Transaction, ListTransactionInternalDto>()
            .Map(dest => dest.Id, src => src.PublicId)
            .Map(dest => dest.Email, src => src.User.Email)
            .Map(dest => dest.Nickname, src => src.User.Nickname)
            .Map(dest => dest.CryptoTokenId, src => src.CryptoToken.PublicId)
            .Map(dest => dest.OrderUid, src => src.TransactionInternal != null ? src.TransactionInternal.OrderUid : null)
            .Map(dest => dest.OrderNumber, src => src.TransactionInternal != null ? src.TransactionInternal.OrderNumber : null)
            .Map(dest => dest.Symbol, src => src.CryptoToken.Symbol)
            .Map(dest => dest.Network, src => src.CryptoToken.Network)
            .Map(dest => dest.ConfirmedAt, src => src.TransactionInternal != null ? src.TransactionInternal.ConfirmedAt : null)
            .Map(dest => dest.ReviewedById, src => src.ReviewedById)
            .Map(dest => dest.ReviewedBy, src => src.ReviewedBy != null ? src.ReviewedBy.UserName : null)
            .Map(dest => dest.DateReviewed, src => src.DateReviewed);

        cfg.NewConfig<Transaction, TransactionInternalDetailDto>()
            .Map(dest => dest.Id, src => src.PublicId)
            .Map(dest => dest.Email, src => src.User.Email)
            .Map(dest => dest.Nickname, src => src.User.Nickname)
            .Map(dest => dest.CryptoTokenId, src => src.CryptoToken.PublicId)
            .Map(dest => dest.Symbol, src => src.CryptoToken.Symbol)
            .Map(dest => dest.Network, src => src.CryptoToken.Network)
            .Map(dest => dest.Hash, src => src.TransactionInternal != null ? src.TransactionInternal.Hash : null)
            .Map(dest => dest.FromAddress, src => src.TransactionInternal != null ? src.TransactionInternal.FromAddress : null)
            .Map(dest => dest.ToAddress, src => src.TransactionInternal != null ? src.TransactionInternal.ToAddress : null)
            .Map(dest => dest.OrderUid, src => src.TransactionInternal != null ? src.TransactionInternal.OrderUid : null)
            .Map(dest => dest.OrderNumber, src => src.TransactionInternal != null ? src.TransactionInternal.OrderNumber : null)
            .Map(dest => dest.ConfirmedAt, src => src.TransactionInternal != null ? src.TransactionInternal.ConfirmedAt : null)
            .Map(dest => dest.ReviewedById, src => src.ReviewedById)
            .Map(dest => dest.ReviewedBy, src => src.ReviewedBy != null ? src.ReviewedBy.UserName : null)
            .Map(dest => dest.DateReviewed, src => src.DateReviewed);

        cfg.NewConfig<Transaction, TransactionInternalDto>()
            .Map(dest => dest.Id, src => src.PublicId)
            .Map(dest => dest.Email, src => src.User.Email)
            .Map(dest => dest.Nickname, src => src.User.Nickname)
            .Map(dest => dest.CryptoTokenId, src => src.CryptoToken.PublicId)
            .Map(dest => dest.Symbol, src => src.CryptoToken.Symbol)
            .Map(dest => dest.Network, src => src.CryptoToken.Network)
            .Map(dest => dest.OrderUid, src => src.TransactionInternal != null ? src.TransactionInternal.OrderUid : null)
            .Map(dest => dest.OrderNumber, src => src.TransactionInternal != null ? src.TransactionInternal.OrderNumber : null)
            .Map(dest => dest.Hash, src => src.TransactionInternal != null ? src.TransactionInternal.Hash : null)
            .Map(dest => dest.FromAddress, src => src.TransactionInternal != null ? src.TransactionInternal.FromAddress : null)
            .Map(dest => dest.ToAddress, src => src.TransactionInternal != null ? src.TransactionInternal.ToAddress : null)
            .Map(dest => dest.ConfirmedAt, src => src.TransactionInternal != null ? src.TransactionInternal.ConfirmedAt : null)
            .Map(dest => dest.ReviewedById, src => src.ReviewedById)
            .Map(dest => dest.ReviewedBy, src => src.ReviewedBy != null ? src.ReviewedBy.UserName : null)
            .Map(dest => dest.DateReviewed, src => src.DateReviewed);

        cfg.NewConfig<TransactionInternalDto, TransactionNotificationDto>()
            .Map(dest => dest.Status, src => src.Status.ToString().ToCamelCase())
            .Map(dest => dest.Type, src => src.Type.ToString().ToCamelCase());

        cfg.NewConfig<TransactionTransferSignalDto, TransactionNotificationDto>()
            .Map(dest => dest.Status, src => src.Status.ToString().ToCamelCase())
            .Map(dest => dest.Type, src => src.Type.ToString().ToCamelCase());

        cfg.NewConfig<TransactionInternalDto, ClientTransactionDto>()
            .Map(dest => dest.TransactionId, src => src.Id)
            .Map(dest => dest.Status, src => src.Status.ToString().ToCamelCase())
            .Map(dest => dest.Type, src => src.Type.ToString().ToCamelCase());

        cfg.NewConfig<TransactionInternalDto, AdminTransactionDto>()
            .Map(dest => dest.Status, src => src.Status.ToString().ToCamelCase())
            .Map(dest => dest.Network, src => src.Network.ToString().ToCamelCase())
            .Map(dest => dest.Type, src => src.Type.ToString().ToCamelCase());

        cfg.NewConfig<CryptoToken, CryptoTokenDto>()
            .Map(dest => dest.Id, src => src.PublicId);

        cfg.NewConfig<Transaction, WalletTransactionDto>()
            .Map(dest => dest.Id, src => src.PublicId)
            .Map(dest => dest.CryptoTokenId, src => src.CryptoToken.PublicId)
            .Map(dest => dest.Symbol, src => src.CryptoToken.Symbol)
            .Map(dest => dest.Network, src => src.CryptoToken.Network)
            .Map(dest => dest.SourceType, src => src.TransactionInternal != null ? WalletSourceType.Internal : WalletSourceType.External)
            .Map(dest => dest.BalanceAfter, src => src.BalanceAfter)
            .Map(dest => dest.ActualAmount, src => src.ActualAmount)
            .Map(dest => dest.GameAmount, src => src.GameAmount)
            .Map(dest => dest.GameBalanceAfter, src => src.GameBalanceAfter)
            .Map(dest => dest.GamePlatformId, src => src.TransactionExternal != null ? src.TransactionExternal.GamePlatform.PublicId : (Guid?)null)
            .Map(dest => dest.GamePlatformName, src => src.TransactionExternal != null ? src.TransactionExternal.GamePlatform.Name : null)
            .Map(dest => dest.From, src => src.TransactionInternal != null ? src.TransactionInternal.FromAddress : null)
            .Map(dest => dest.To, src => src.TransactionInternal != null ? src.TransactionInternal.ToAddress : null);
        
        cfg.NewConfig<TransactionTransferDto, TransactionTransferSignalDto>()
            .Map(dest => dest.Network, src => src.Network.ToString().ToCamelCase())
            .Map(dest => dest.Type, src => src.Type.ToString().ToCamelCase())
            .Map(dest => dest.Status, src => src.Status.ToString().ToCamelCase())
            .Map(dest => dest.SourceType, src => src.SourceType.ToString().ToCamelCase())
            ;
    }
}