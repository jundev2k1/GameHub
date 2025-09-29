using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.SignalR.Dtos;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.LiveStreams.Streaming.Dtos;
using game_x.application.Utils;
using System.Text.Json;

namespace game_x.application.Events.OnLiveStreamDonated;

public sealed class OnLiveStreamDonatedHandler(
    IUnitOfWork unitOfWork,
    IUserBalanceRepo userBalanceRepo,
    ITransactionRepo transactionRepo,
    INotificationRepo notificationRepo,
    ILiveStreamDonationRepo liveStreamDonationRepo,
    ILiveStreamManagerCacheService liveStreamManager,
    IClientHubService clientHub,
    ILiveStreamHubService liveStreamHub) : IApplicationEventHandler<OnLiveStreamDonatedEvent>
{
    public async Task Handle(OnLiveStreamDonatedEvent @event, CancellationToken ct = default)
    {
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await userBalanceRepo.UpdateAsync(@event.UserBalanceId, ub =>
            {
                ub.AdjustAmount(@event.Amount, false);
            }, ct);

            await CreateTransaction(@event.Amount, @event.UserId, feeAmount: 0, @event.CryptoId, ct);
            await CreateDonation(@event.StreamInfo, @event.Amount, @event.UserId, @event.Message, @event.GiftId, ct);
            await CreateNotificationForDoner(@event.UserId, this.StreamDonation!, ct);
            await CreateNotificationForStreamer(@event.StreamInfo.AssignedTo!.Id, this.StreamDonation!, ct);
        }, ct);

        // Notify doner
        if (this.DonerNotification != null)
            await clientHub.SendNotificationToMemberAsync(@event.UserId, this.DonerNotification);

        if (this.TalentNotification != null)
            await clientHub.SendNotificationToMemberAsync(@event.StreamInfo.AssignedTo!.Id, this.TalentNotification);

        if (this.StreamDonation != null)
        {
            liveStreamManager.AddDonationToStream(@event.StreamInfo.StreamKey, this.StreamDonation);
            await liveStreamHub.NotifyDonationCompleted(@event.StreamInfo.StreamKey, this.StreamDonation);
        }
    }

    private async Task CreateTransaction(
        decimal amount,
        string userId,
        decimal feeAmount,
        int tokenId,
        CancellationToken ct = default)
    {
        var transaction = Transaction.Create(
            sourceType: TransactionSourceType.GameX,
            type: TransactionType.Transfer,
            userId: userId,
            amount: amount,
            fee: feeAmount,
            cryptoTokenId: tokenId,
            note: "Livestream donations.");
        var orderNumber = await OrderNoGenerator.GenerateUniqueOtcOrderNoAsync(transactionRepo, ct);
        var transactionInternal = TransactionInternal.Create(
            orderNumber: orderNumber,
            fromAddress: string.Empty,
            toAddress: string.Empty);
        transaction.AddTxInternal(transactionInternal);

        await transactionRepo.AddAsync(transaction, ct);
    }

    private async Task CreateDonation(
        LiveStreamStatusDto streamInfo,
        decimal amount,
        string userId,
        string message = "",
        int? giftId = null,
        CancellationToken ct = default)
    {
        var donation = LiveStreamDonation.Create(
            streamInfo.LocalId,
            userId,
            message,
            amount,
            giftId);
        await liveStreamDonationRepo.CreateAsync(donation, ct);
        this.StreamDonation = donation.Adapt<LiveStreamDonationDto>();
    }

    private async Task CreateNotificationForDoner(
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
        this.DonerNotification = notification.Adapt<NotificationDto>();
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

    private NotificationDto? DonerNotification { get; set; }
    private NotificationDto? TalentNotification { get; set; }
    private LiveStreamDonationDto? StreamDonation { get; set; }
}
