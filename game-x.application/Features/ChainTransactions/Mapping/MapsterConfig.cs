using game_x.application.Contract.Infrastructure.SignalR.Dtos.Transactions;
using game_x.application.Features.ChainTransactions.Dtos;
using game_x.application.Features.ChainTransactions.Shared.Queries.GetCryptoTokenList;

namespace game_x.application.Features.ChainTransactions.Mapping;

public sealed class MapsterConfig : IRegister
{
    public void Register(TypeAdapterConfig cfg)
    {
        cfg.NewConfig<Transaction, ListTransactionInternalDto>()
            .Map(dest => dest.Id, src => src.PublicId)
            .Map(dest => dest.CryptoTokenId, src => src.CryptoToken.PublicId)
            .Map(dest => dest.OrderUid, src => src.TransactionInternal != null ? src.TransactionInternal.OrderUid : null)
            .Map(dest => dest.OrderNumber, src => src.TransactionInternal != null ? src.TransactionInternal.OrderNumber : null)
            .Map(dest => dest.Symbol, src => src.CryptoToken.Symbol)
            .Map(dest => dest.Network, src => src.CryptoToken.Network)
            .Map(dest => dest.ConfirmedAt, src => src.TransactionInternal != null ? src.TransactionInternal.ConfirmedAt : null);
        
        cfg.NewConfig<Transaction, TransactionInternalDetailDto>()
            .Map(dest => dest.Id, src => src.PublicId)
            .Map(dest => dest.CryptoTokenId, src => src.CryptoToken.PublicId)
            .Map(dest => dest.Symbol, src => src.CryptoToken.Symbol)
            .Map(dest => dest.Network, src => src.CryptoToken.Network)
            .Map(dest => dest.Hash, src => src.TransactionInternal != null ? src.TransactionInternal.Hash : null)
            .Map(dest => dest.FromAddress, src => src.TransactionInternal != null ? src.TransactionInternal.FromAddress : null)
            .Map(dest => dest.ToAddress, src => src.TransactionInternal != null ? src.TransactionInternal.ToAddress : null)
            .Map(dest => dest.OrderUid, src => src.TransactionInternal != null ? src.TransactionInternal.OrderUid : null)
            .Map(dest => dest.OrderNumber, src => src.TransactionInternal != null ? src.TransactionInternal.OrderNumber : null)
            .Map(dest => dest.ConfirmedAt, src => src.TransactionInternal != null ? src.TransactionInternal.ConfirmedAt : null);
        
        cfg.NewConfig<Transaction, TransactionInternalDto>()
            .Map(dest => dest.Id, src => src.PublicId)
            .Map(dest => dest.CryptoTokenId, src => src.CryptoToken.PublicId)
            .Map(dest => dest.Symbol, src => src.CryptoToken.Symbol)
            .Map(dest => dest.Network, src => src.CryptoToken.Network)
            .Map(dest => dest.OrderUid, src => src.TransactionInternal != null ? src.TransactionInternal.OrderUid : null)
            .Map(dest => dest.OrderNumber, src => src.TransactionInternal != null ? src.TransactionInternal.OrderNumber : null)
            .Map(dest => dest.Hash, src => src.TransactionInternal != null ? src.TransactionInternal.Hash : null)
            .Map(dest => dest.FromAddress, src => src.TransactionInternal != null ? src.TransactionInternal.FromAddress : null)
            .Map(dest => dest.ToAddress, src => src.TransactionInternal != null ? src.TransactionInternal.ToAddress : null)
            .Map(dest => dest.ConfirmedAt, src => src.TransactionInternal != null ? src.TransactionInternal.ConfirmedAt : null);
        
        cfg.NewConfig<TransactionInternalDto, TransactionNotificationDto>()
            .Map(dest => dest.Status, src => src.Status.ToString().ToLower())
            .Map(dest => dest.Type, src => src.Type.ToString().ToLower());
        
        cfg.NewConfig<TransactionInternalDto, ClientTransactionDto>()
            .Map(dest => dest.TransactionId, src => src.Id)
            .Map(dest => dest.Status, src => src.Status.ToString().ToLower())
            .Map(dest => dest.Type, src => src.Type.ToString().ToLower());
        
        cfg.NewConfig<TransactionInternalDto, AdminTransactionDto>()
            .Map(dest => dest.Status, src => src.Status.ToString().ToLower())
            .Map(dest => dest.Network, src => src.Network.ToString().ToLower())
            .Map(dest => dest.Type, src => src.Type.ToString().ToLower());
        
        cfg.NewConfig<CryptoToken, CryptoTokenDto>()
            .Map(dest => dest.Id, src => src.PublicId);
    }
}