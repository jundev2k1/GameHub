using System.Text.Json;
using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Infrastructure.SignalR.Dtos;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Accounts.User.Dtos;

namespace game_x.application.Events.OnVerifyUpdated;

public sealed class OnVerifyUpdatedHandler(
    IUnitOfWork unitOfWork,
    INotificationRepo notificationRepo,
    IClientHubService clientHubService) : IApplicationEventHandler<OnVerifyUpdatedEvent>
{
    public async Task Handle(OnVerifyUpdatedEvent @event, CancellationToken ct = default)
    {
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await SendToMember(@event.UserId, @event.VerificationStatus, ct);
        }, ct);
    }

    private async Task SendToMember(string userId, VerificationStatusDto verificationDto, CancellationToken ct)
    {
        var notificationDto = new VerificationNotificationDto
        {
            CurrencyCode = verificationDto.CurrencyCode,
            Type = verificationDto.Type.ToString().ToLower(),
            Status = verificationDto.Status.ToString().ToLower(),
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
    }
}