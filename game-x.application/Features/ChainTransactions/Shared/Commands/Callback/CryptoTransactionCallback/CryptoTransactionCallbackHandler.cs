using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.OnUserBalanceChanged;
using game_x.application.Events.OnUxmTransactionCallback;
using game_x.share.ExternalApi.Uxm.Dtos;

namespace game_x.application.Features.ChainTransactions.Shared.Commands.Callback.CryptoTransactionCallback;

public sealed class CryptoTransactionCallbackHandler(
    IAsymmetricCryptoService asymmetricCryptoService,
    IAsymmetricKeyCacheService asymmetricKeyCacheService,
    IUserBalanceRepo userBalanceRepo,
    ICryptoTokenRepo cryptoTokenRepo,
    IChainTransactionRepo chainTransactionRepo,
    IApplicationEventDispatcher eventDispatcher) : ICommandHandler<CryptoTransactionCallbackCommand, Unit>
{
    public async Task<Unit> Handle(CryptoTransactionCallbackCommand request,
        CancellationToken ct = default)
    {
        var (requestData, signature) = request;

        // Verify UXM signature
        var uxmPublicKey = asymmetricKeyCacheService.UxmPublicKey;
        var isValid = asymmetricCryptoService.VerifySignature(uxmPublicKey, requestData, signature);
        if (!isValid)
            throw new BadRequestException(MessageCode.System.TokenGenerationFailed, "Invalid signature.");

        await eventDispatcher.Publish(new OnUxmTransactionCallbackEvent(
            OrderUid: requestData.OrderUid,
            Hash: requestData.Hash,
            OrderNumber: requestData.OrderNumber,
            ActualAmount: requestData.ActualAmount,
            CreatedAt: requestData.CreatedAt,
            ConfirmedAt: requestData.ConfirmedAt,
            Remark: requestData.Remark), ct);

        var userBalance = await LoadAndValidateUserBalanceAsync(requestData, ct);

        await eventDispatcher.Publish(new OnUserBalanceChangedEvent(userBalance), ct);

        return Unit.Value;
    }
    private async Task<UserBalance> LoadAndValidateUserBalanceAsync(CryptoCallbackRequest requestData, CancellationToken ct)
    {
        if (requestData is null || string.IsNullOrWhiteSpace(requestData.OrderNumber))
            throw new BadRequestException("OrderNumber is required");

        var transaction = await chainTransactionRepo.GetByOrderNumberAsync(requestData.OrderNumber, ct)
            ?? throw new BadRequestException(MessageCode.Transaction.ChainTransactionNotFound);

        if (string.IsNullOrWhiteSpace(transaction.UserId))
            throw new BadRequestException("User ChainTransaction is required");

        var token = await cryptoTokenRepo.GetBySymbolAndNetworkAsync(CryptoTokenSymbol.Usdt, NetworkType.Tron, ct)
            ?? throw new BadRequestException(MessageCode.Crypto.CryptoTokenNotFound);

        return await userBalanceRepo.GetByUserIdAndTokenIdAsync(transaction.UserId, token.Id, ct)
            ?? throw new BadRequestException(MessageCode.Accounting.BalanceNotFound);
    }
}
