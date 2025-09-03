using System.Text.Json;
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
            await SendToAdmin(@event, ct);
        }, ct);
    }

    private async Task SendToAdmin(OnVerifyCreatedEvent @event, CancellationToken ct)
    {
        var adminUsers = await userRepo.GetAdminUsers(ct);
        var userId = @event.UserId;
        var user = await userRepo.GetUserByIdAsync(userId, ct);

        foreach (var adminUser in adminUsers)
        {
            if (@event.VerificationStatusType == VerificationStatusType.Kyc && @event.UserKycDto != null)
            {
                await SendKycToAdmin(@event, user, adminUser, ct);
            }
            else if (@event.VerificationStatusType == VerificationStatusType.BankAccount && @event.BankAccountDto != null)
            {
                await SendBankAccountToAdmin(@event, user, adminUser, ct);
            }
        }
    }

    private async Task SendKycToAdmin(OnVerifyCreatedEvent @event, User user, User adminUser, CancellationToken ct)
    {
        var verificationDto = new AdminVerificationCreatedDto
        {
            VerifyType = VerificationStatusType.Kyc.ToString().ToCamelCase(),
            Email = user.Email ?? string.Empty,
            NickName = user.Nickname ?? string.Empty,
        };
        var notification = Notification.Create(
            NotificationMessageKey.User_Verify_Created,
            adminUser.Id,
            NotificationType.Info,
            NotificationSeverity.Info,
            JsonSerializer.Serialize(verificationDto));
        await notificationRepo.AddNotificationAsync(notification, ct);
        await adminHubService.SendNotificationAsync(
            adminUser.Id,
            notification.Adapt<NotificationDto>());
        if (@event.UserKycDto != null)
        {
            await adminHubService.SendVerificationToAdminAsync(adminUser.Id, @event.UserKycDto);
        }
    }

    private async Task SendBankAccountToAdmin(OnVerifyCreatedEvent @event, User user, User adminUser, CancellationToken ct)
    {
        var verificationDto = new AdminVerificationCreatedDto
        {
            VerifyType = VerificationStatusType.BankAccount.ToString().ToCamelCase(),
            Email = user.Email ?? string.Empty,
            NickName = user.Nickname ?? string.Empty,
            AccountName = @event.BankAccountDto?.AccountName ?? string.Empty,
        };
        var notification = Notification.Create(
            NotificationMessageKey.User_Verify_Created,
            adminUser.Id,
            NotificationType.Info,
            NotificationSeverity.Info,
            JsonSerializer.Serialize(verificationDto));
        await notificationRepo.AddNotificationAsync(notification, ct);
        await adminHubService.SendNotificationAsync(
            adminUser.Id,
            notification.Adapt<NotificationDto>());
        if (@event.BankAccountDto != null)
        {
            await adminHubService.SendVerificationToAdminAsync(adminUser.Id, @event.BankAccountDto);
        }
    }
}
