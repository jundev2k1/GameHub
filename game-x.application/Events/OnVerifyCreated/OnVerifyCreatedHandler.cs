using System.Text.Json;
using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Infrastructure.SignalR.Dtos;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Accounts.User.Dtos;
using game_x.share.Extensions;

namespace game_x.application.Events.OnVerifyCreated;

public sealed class OnVerifyCreatedHandler(
    IUnitOfWork unitOfWork,
    IUserRepo userRepo,
    INotificationRepo notificationRepo,
    IAdminHubService adminHubService) : IApplicationEventHandler<OnVerifyCreatedEvent>
{
    public async Task Handle(OnVerifyCreatedEvent @event, CancellationToken ct = default)
    {
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await SendToAdmin(@event.VerificationCreated, ct);

        }, ct);
    }

    private async Task SendToAdmin(VerificationCreatedDto verificationDto, CancellationToken ct)
    {
        var adminUsers = await userRepo.GetAdminUsers(ct);

        foreach (var adminUser in adminUsers)
        {
            var notificationDto = new VerificationCreatedDto
            {
                Type = verificationDto.Type.ToCamelCase(),
                Email = verificationDto.Email,
                NickName = verificationDto.NickName
            };

            var notification = Notification.Create(
                NotificationMessageKey.User_Verify_Created,
                adminUser.Id,
                NotificationType.Info,
                NotificationSeverity.Info,
                JsonSerializer.Serialize(notificationDto));
            await notificationRepo.AddNotificationAsync(notification, ct);

            await adminHubService.SendNotificationAsync(
                adminUser.Id,
                notification.Adapt<NotificationDto>());
        }
    }
}
