using game_x.application.Contract.Infrastructure.Services.Statistics.Admin;
using game_x.application.Contract.Infrastructure.SignalR.Dtos;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Notification;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Accounts.User.Dtos;
using game_x.share.Extensions;
using System.Text.Json;

namespace game_x.application.Events.Account.OnVerifyUpdated;

public sealed class OnVerifyUpdatedHandler(
    IUnitOfWork unitOfWork,
    INotificationRepo notificationRepo,
    IAdminStatistics adminStatistics,
    IClientHubService clientHubService,
    IAdminHubService adminHubService,
    ICsAdminHubService csAdminHubService) : IApplicationEventHandler<OnVerifyUpdatedEvent>
{
    public async Task Handle(OnVerifyUpdatedEvent @event, CancellationToken ct = default)
    {
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await SendToMember(@event.UserId, @event.PublicId, @event.VerificationStatus, ct);
        }, ct);
    }

    private async Task SendToMember(string userId, Guid publicId, VerificationStatusDto verificationDto, CancellationToken ct)
    {
        var notificationDto = new VerificationNotificationDto
        {
            CurrencyCode = verificationDto.CurrencyCode,
            Type = verificationDto.Type.ToCamelCase(),
            Status = verificationDto.Status.ToCamelCase(),
            IsVerified = verificationDto.IsVerified
        };

        var notification = Notification.Create(
            NotificationMessageKey.User_VerifyStatus_Changed,
            userId,
            NotificationType.Info,
            NotificationSeverity.Info,
            JsonSerializer.Serialize(notificationDto));
        await notificationRepo.AddNotificationAsync(notification, ct);

        await clientHubService.SendNotificationToMemberAsync(
            userId,
            notification.Adapt<NotificationDto>());

        await clientHubService.SendVerifyUpdateAsync(
            userId,
            verificationDto);

        await SendOrderReviewedForAdminAsync(publicId, verificationDto.Type, verificationDto.Status, ct);
    }

    private async Task SendOrderReviewedForAdminAsync(Guid publicId, VerificationStatusType type, VerificationStatus status, CancellationToken ct)
    {
        var (withdrawalCount, kycCount, bankAccountCount) = await adminStatistics.GetUnderReviewStatisticsAsync(ct);
        var dto = new AdminOrderReviewedDto
        {
            Id = publicId,
            Status = status.ToCamelCase()
        };
        switch (type)
        {
            case VerificationStatusType.Kyc:
                dto.UnderReviewCount = kycCount;
                await adminHubService.NotifyOrderKycReviewedToAdminAsync(dto);
                await csAdminHubService.NotifyOrderKycReviewedToOneAsync(dto);
                break;

            case VerificationStatusType.BankAccount:
                dto.UnderReviewCount += bankAccountCount;
                await adminHubService.NotifyOrderBankAccountReviewedToAdminAsync(dto);
                await csAdminHubService.NotifyOrderBankAccountReviewedToOneAsync(dto);
                break;
        }
    }
}
