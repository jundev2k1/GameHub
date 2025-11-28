using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.SignalR.Dtos;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.OnUserBalanceUpdated;
using game_x.application.Features.LiveStreams.Streaming.Dtos;
using game_x.application.Utils;
using System.Text.Json;
using static game_x.share.Helper.CursorHelper;

namespace game_x.application.Events.OnLiveStreamDonated;

public sealed class OnLiveStreamDonatedHandler(
    IUnitOfWork unitOfWork,
    IUserRepo userRepo,
    IUserBalanceRepo userBalanceRepo,
    ITalentWalletRepo talentWalletRepo,
    ITransactionRepo transactionRepo,
    INotificationRepo notificationRepo,
    ILiveStreamChatRepo liveStreamChatRepo,
    ILiveStreamDonationRepo liveStreamDonationRepo,
    ILiveStreamManagerCacheService liveStreamManager,
    IClientHubService clientHub,
    ILiveStreamHubService liveStreamHub,
    IApplicationEventDispatcher eventDispatcher) : IApplicationEventHandler<OnLiveStreamDonatedEvent>
{
    public async Task Handle(OnLiveStreamDonatedEvent @event, CancellationToken ct = default)
    {
        var donorInfo = await userRepo.GetUserByIdAsync(@event.UserId, ct);
        await unitOfWork.WithTransactionAsync(async () =>
        {
            // Decrease user balance
            await userBalanceRepo.UpdateAsync(@event.UserBalanceId, ub =>
            {
                ub.AdjustAmount(@event.Amount, false);
            }, ct);
            // Increase talent balance
            await talentWalletRepo.UpdateAsync(@event.StreamInfo.AssignedTo!.Id, talentWallet =>
            {
                var newBalance = talentWallet.Balance + @event.Amount;
                talentWallet.AdjustBalance(newBalance);

                var transaction = Transaction.Create(
                    sourceType: TransactionSourceType.GameX,
                    type: type,
                    userId: userId,
                    amount: amount,
                    fee: feeAmount,
                    cryptoTokenId: tokenId,
                    note: "Livestream donations.");
                var tx = TalentWalletTransaction.Create(
                    @event.StreamInfo.AssignedTo!.Id,
                    TalentTransactionType.Commission);
            }, ct);

            await CreateTransaction(TransactionType.TransferSent, @event.Amount, @event.UserId, feeAmount: 0, @event.CryptoId, ct);
            await CreateTransaction(TransactionType.TransferReceived, @event.Amount, @event.StreamInfo.AssignedTo!.Id, feeAmount: 0, @event.CryptoId, ct);
            await CreateDonation(@event.StreamInfo, @event.Amount, donorInfo, @event.Message, @event.Gift, ct);
            await CreateStreamMessage(@event.StreamInfo, @event.Amount, donorInfo, @event.Message, @event.Gift);
            await CreateNotificationForDonor(@event.UserId, this.StreamDonation!, ct);
            await CreateNotificationForStreamer(@event.StreamInfo.AssignedTo!.Id, this.StreamDonation!, ct);
        }, ct);

        // Notify donor
        if (this.DonorNotification != null)
            await clientHub.SendNotificationToMemberAsync(@event.UserId, this.DonorNotification);

        // Notify talent
        if (this.TalentNotification != null)
            await clientHub.SendNotificationToMemberAsync(@event.StreamInfo.AssignedTo!.Id, this.TalentNotification);

        // Notify stream
        if (this.StreamDonation != null)
        {
            liveStreamManager.AddDonationToStream(@event.StreamInfo.StreamKey, this.StreamDonation);
            await liveStreamHub.NotifyDonationCompleted(@event.StreamInfo.StreamKey, this.StreamDonation);
        }

        // Notify chat message for stream
        if (this.ChatMessage != null)
        {
            liveStreamManager.AddMessageToStream(@event.StreamInfo.StreamKey, this.ChatMessage);
            await liveStreamHub.SendChatMessage(@event.StreamInfo.StreamKey, this.ChatMessage);
        }

        // Refresh balance for talent and donor
        if (@event.StreamInfo.AssignedTo!.Id != @event.UserId)
        {
            var talentBalanceChangeEvent = new OnUserBalanceUpdatedEvent(@event.StreamInfo.AssignedTo.Id);
            await eventDispatcher.Publish(talentBalanceChangeEvent, ct);
            var donorBalanceChangeEvent = new OnUserBalanceUpdatedEvent(@event.UserId);
            await eventDispatcher.Publish(donorBalanceChangeEvent, ct);
        }
    }

    private async Task CreateTransaction(
        TransactionType type,
        decimal amount,
        string userId,
        decimal feeAmount,
        int tokenId,
        CancellationToken ct = default)
    {
        var transaction = Transaction.Create(
            sourceType: TransactionSourceType.GameX,
            type: type,
            userId: userId,
            amount: amount,
            fee: feeAmount,
            cryptoTokenId: tokenId,
            note: "Livestream donations.");
        var orderNumber = await OrderNoGenerator.GenerateUniqueOtcOrderNoAsync(transactionRepo, ct);
        var lastedBalanceAfter = await transactionRepo.GetLatestBalanceAfterAsync(transaction.UserId, ct);
        var transactionInternal = TransactionInternal.Create(
            orderNumber: orderNumber,
            fromAddress: string.Empty,
            toAddress: string.Empty);
        transaction.AddTxInternal(transactionInternal);

        var balanceAfter = type switch
        {
            TransactionType.TransferSent => lastedBalanceAfter - amount,
            TransactionType.TransferReceived => lastedBalanceAfter + amount,
            _ => throw new NotSupportedException("Invalid type.")
        };
        transaction.ConfirmTx(amount, balanceAfter, DateTime.UtcNow);

        await transactionRepo.AddAsync(transaction, ct);
    }

    private async Task CreateDonation(
        LiveStreamStatusDto streamInfo,
        decimal amount,
        User donor,
        string message = "",
        LiveStreamGift? gift = null,
        CancellationToken ct = default)
    {
        var donation = LiveStreamDonation.Create(
            streamInfo.LocalId,
            donor.Id,
            message,
            amount,
            gift?.Id);

        await liveStreamDonationRepo.CreateAsync(donation, ct);
        this.StreamDonation = donation.Adapt<LiveStreamDonationDto>();
        this.StreamDonation.DonorName = donor.Nickname;
        this.StreamDonation.LivestreamScheduleId = streamInfo.Id;
        this.StreamDonation.GiftId = gift?.PublicId;
    }

    private async Task CreateStreamMessage(
        LiveStreamStatusDto streamInfo,
        decimal amount,
        User donor,
        string message,
        LiveStreamGift? gift = null)
    {
        string? giftSnapshot = null;
        if (gift != null)
        {
            var giftInfo = new Dictionary<string, string>()
            {
                { "giftName", gift.Name }
            };
            giftSnapshot = JsonSerializer.Serialize(giftInfo);
        }

        var chatMessage = LiveStreamChatMessage.Create(
            Guid.NewGuid(),
            streamInfo.LocalId,
            donor.Id,
            message.Trim(),
            LiveStreamChatMessageType.Donation,
            amount,
            giftSnapshot);
        await liveStreamChatRepo.CreateAsync(chatMessage);

        this.ChatMessage = new LiveStreamChatMessageDto()
        {
            Id = chatMessage.PublicId,
            StreamId = streamInfo.Id,
            Nickname = donor.Nickname,
            DonationAmount = amount,
            IsHost = donor.Id == streamInfo.AssignedTo?.Id,
            Message = chatMessage.Message,
            MessageType = chatMessage.MessageType,
            SenderId = chatMessage.SenderId,
            DeleteReason = chatMessage.DeleteReason,
            IsDeleted = chatMessage.IsDeleted,
            SentAt = chatMessage.SentAt,
            Metadata = giftSnapshot
        };
    }

    private async Task CreateNotificationForDonor(
        string userId,
        LiveStreamDonationDto donationDto,
        CancellationToken ct = default)
    {
        var notification = Notification.Create(
            NotificationMessageKey.LiveStream_DonationSuccessful,
            userId,
            NotificationType.LiveStream,
            NotificationSeverity.Info,
            JsonSerializer.Serialize(donationDto));
        await notificationRepo.AddNotificationAsync(notification, ct);
        this.DonorNotification = notification.Adapt<NotificationDto>();
    }

    private async Task CreateNotificationForStreamer(
        string streamerId,
        LiveStreamDonationDto donationDto,
        CancellationToken ct = default)
    {
        var notification = Notification.Create(
            NotificationMessageKey.LiveStream_DonationReceived,
            streamerId,
            NotificationType.LiveStream,
            NotificationSeverity.Info,
            JsonSerializer.Serialize(donationDto));
        await notificationRepo.AddNotificationAsync(notification, ct);
        this.TalentNotification = notification.Adapt<NotificationDto>();
    }

    private NotificationDto? DonorNotification { get; set; }
    private NotificationDto? TalentNotification { get; set; }
    private LiveStreamDonationDto? StreamDonation { get; set; }
    private LiveStreamChatMessageDto? ChatMessage { get; set; }
}
