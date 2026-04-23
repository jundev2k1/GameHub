using System.Globalization;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Transactions;
using game_x.application.Contract.Persistence.Identity;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.Transaction.OnTransactionTransferred;
using game_x.application.Events.Account.OnUserBalanceUpdated;
using game_x.application.Features.Chat.Commands.SendMessage;
using Microsoft.Extensions.Logging;

namespace game_x.application.Features.Transactions.Client.Commands.TraceV1.TransferBetweenFriends;

public sealed class CreateDepositChainTransactionHandler(
    IUserAccessor userAccessor,
    IUnitOfWork unitOfWork,
    IUserRepo userRepo,
    ISocialLinkRepo socialLinkRepo,
    ICryptoTokenRepo cryptoTokenRepo,
    ITransactionRepo transactionRepo,
    IUserBalanceRepo userBalanceRepo,
    IConversationService convService,
    ISender sender,
    IApplicationEventDispatcher dispatcher,
    ILogger<CreateDepositChainTransactionHandler> logger) : ICommandHandler<TransferBetweenFriendsCommand, Unit>
{
    public async Task<Unit> Handle(TransferBetweenFriendsCommand cmd, CancellationToken ct)
    {
        await unitOfWork.WithTransactionAsync(async () =>
        {
            try
            {
                var me = userAccessor.GetUserId();
                await ValidateFriendAsync(me, cmd.TargetUserId, ct);
                var (transferorBalance, receiverBalance) = await ValidateBalanceAsync(me, cmd.TargetUserId, cmd.Amount, ct);
                var (outgoingTx, incomingTx) = await CreateTransactionsAsync(
                    cmd: cmd,
                    tokenId: receiverBalance.CryptoTokenId,
                    transferorId: me,
                    receiverId: cmd.TargetUserId,
                    ct: ct);

                await userBalanceRepo.UpdateAsync(transferorBalance.PublicId, x =>
                {
                    x.AdjustAmount(outgoingTx.Amount, false);
                }, ct);
            
                await userBalanceRepo.UpdateAsync(receiverBalance.PublicId, x =>
                {
                    x.AdjustAmount(outgoingTx.Amount, true);
                }, ct);
                await unitOfWork.SaveChangesAsync(ct);
                outgoingTx.CompleteTransfer(transferorBalance.TotalAmount, incomingTx.Id);
                incomingTx.CompleteTransfer(receiverBalance.TotalAmount, outgoingTx.Id);
                
                await unitOfWork.SaveChangesAsync(ct);
                await CreateTransferMessageAsync(me, cmd.TargetUserId, cmd.Amount, ct);
                await SendSignalsAsync(me, cmd.TargetUserId, outgoingTx.PublicId, incomingTx.PublicId, ct);
            }
            catch(Exception ex)
            {
                logger.LogError(ex, ex.Message);
                await unitOfWork.RollbackAsync(ct);
                throw;
            }
        }, ct);
        return Unit.Value;
    }

    private async Task ValidateFriendAsync(string transferorId, string receiverId, CancellationToken ct)
    {
        if (transferorId == receiverId) throw new BadRequestException(MessageCode.Chatting.FailToTargetMyself);

        var isExistedTargetUser = await userRepo.IsExistUserIdAsync(receiverId, ct);
        if(!isExistedTargetUser)
            throw new NotFoundException(MessageCode.User.UserNotFound);
        
        var (min, max) = SocialLinkPair.Normalize(transferorId, receiverId);
        var existed = await socialLinkRepo.GetByKeyPairAsync(min, max, ct);
        
        if (existed is null || !existed.IsFriend) 
            throw new BadRequestException(MessageCode.Chatting.StillNotFriend);
    }
    
    private async Task<(UserBalance transferorBalance, UserBalance receiverBalance)> 
        ValidateBalanceAsync(string transferorId, string receiverId, decimal amount, CancellationToken ct)
    {
        var token = await cryptoTokenRepo.GetBySymbolAndNetworkAsync(CryptoTokenSymbol.Usdt, NetworkType.Tron, ct);
        if (token == null || token.Status != CryptoTokenStatus.Active)
            throw new BadRequestException(MessageCode.Crypto.CryptoTokenUnsupported);
        
        var transferorBalance = await GetUserBalanceAsync(transferorId, token.Id, ct);
        if (transferorBalance.Amount < amount)
            throw new BadRequestException(MessageCode.Accounting.InsufficientBalance);
        
        var receiverBalance = await GetUserBalanceAsync(receiverId, token.Id, ct);
        return (transferorBalance, receiverBalance);
    }
    
    private async Task<(Transaction transferredTx, Transaction receivedTx)> CreateTransactionsAsync(
        TransferBetweenFriendsCommand cmd,
        int tokenId,
        string transferorId,
        string receiverId,
        CancellationToken ct)
    {
        async Task IncludeTxInternal(Transaction tx)
        {
            var internalTx = TransactionInternal.Create(
                sourceType: TransactionSourceType.Transfer);
            tx.AddTxInternal(internalTx);
            await transactionRepo.AddAsync(tx, ct);
        }
        
        var outgoingTx = Transaction.Create(
            type: TransactionType.TransferSent,
            userId: transferorId,
            amount: cmd.Amount,
            cryptoTokenId: tokenId,
            note: cmd.Note);
        
        var incomingTx = Transaction.Create(
            type: TransactionType.TransferReceived,
            userId: receiverId,
            amount: cmd.Amount,
            cryptoTokenId: tokenId,
            note: cmd.Note);
        
        await Task.WhenAll(
            IncludeTxInternal(outgoingTx),
            IncludeTxInternal(incomingTx));

        return (outgoingTx, incomingTx);
    }
    
    private async Task<UserBalance> GetUserBalanceAsync(string userId, int tokenId, CancellationToken ct)
    {
        return await userBalanceRepo.GetByUserIdAndTokenIdAsync(userId, tokenId, ct)
            ?? throw new BadRequestException(MessageCode.Accounting.BalanceNotFound);
    }
    
    private async Task CreateTransferMessageAsync(
        string me, 
        string peerUserId,
        decimal amount,
        CancellationToken ct)
    {
       var conv = await convService.EnsureForPair(me, peerUserId, ct);
       await sender.Send(new SendMessageCommand
       (
           ConversationId: conv.PublicId,
           IsAgent: false,
           SenderActorId: me,
           Text: amount.ToString("F2", CultureInfo.InvariantCulture),
           ClientLocalId: Guid.NewGuid().ToString(),
           SenderUserId: peerUserId,
           Kind: MessageKind.SystemTransfer
       ), ct);
    }
    
    private async Task SendSignalsAsync(
        string transferorId, 
        string receiverId,
        Guid transferTxId,
        Guid receivedTxId,
        CancellationToken ct)
    {
        var updatedTransferTx = await transactionRepo.GetTransferByIdAsync(transferTxId, ct);
        var updatedReceivedTx = await transactionRepo.GetTransferByIdAsync(receivedTxId, ct);
        await dispatcher.Publish(new OnTransactionTransferredEvent(updatedTransferTx, updatedTransferTx.Adapt<TransactionTransferSignalDto>() with {Amount = - updatedTransferTx.Amount}), ct);
        await dispatcher.Publish(new OnTransactionTransferredEvent(updatedReceivedTx, updatedReceivedTx.Adapt<TransactionTransferSignalDto>()), ct);
        await dispatcher.Publish(new OnUserBalanceUpdatedEvent(transferorId), ct);
        await dispatcher.Publish(new OnUserBalanceUpdatedEvent(receiverId), ct);
    }
}