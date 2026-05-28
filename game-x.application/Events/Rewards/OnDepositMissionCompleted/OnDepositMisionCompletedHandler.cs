using System.Text.Json;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Notification;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Accounts.User.Commands.DailyCheckIn;
using Microsoft.Extensions.Logging;

namespace game_x.application.Events.Rewards.OnDepositMissionCompleted;

public sealed class OnDepositMissionCompletedHandler(
    IUnitOfWork unitOfWork,
    INotificationRepo notificationRepo,
    IClientHubService clientHubService,
    ILogger<DailyCheckInHandler> logger) : IApplicationEventHandler<OnDepositMissionCompletedEvent>
{
    public async Task Handle(OnDepositMissionCompletedEvent @event, CancellationToken ct = default)
    {
        await unitOfWork.WithTransactionAsync(async () =>
        {
            try
            {
                var notification = Notification.Create(
                    NotificationMessageKey.Misssion_Deposit_Completed,
                    @event.UserId,
                    NotificationType.Mission,
                    NotificationSeverity.Success,
                    JsonSerializer.Serialize(@event.Dto));
                
                await notificationRepo.AddNotificationAsync(notification, ct);
                await unitOfWork.CommitAsync(ct);
                
                await clientHubService.SendNotificationToMemberAsync(@event.UserId, notification.Adapt<NotificationDto>());
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
            }
        }, ct);
    }
}