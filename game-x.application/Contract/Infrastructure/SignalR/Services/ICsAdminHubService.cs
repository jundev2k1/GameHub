using game_x.application.Contract.Infrastructure.SignalR.Dtos;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Notification;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Transactions;
using game_x.application.Features.BankAccountVerifications.Dtos;
using game_x.application.Features.Kyc.Dtos;

namespace game_x.application.Contract.Infrastructure.SignalR.Services;

public interface ICsAdminHubService
{
    Task SendNotificationToOneAsync(string adminId, NotificationDto message);

    Task SendNotificationToAllAsync(NotificationDto message);

    Task SendTransactionToOneAsync(string adminId, AdminTransactionDto transaction);

    Task SendVerificationToOneAsync(string adminId, UserKycListItemDto message);

    Task SendVerificationToOneAsync(string adminId, BankAccountListItemDto message);

    Task NotifyOrderTxReviewedToOneAsync(AdminOrderReviewedDto order);

    Task NotifyOrderKycReviewedToOneAsync(AdminOrderReviewedDto order);

    Task NotifyOrderBankAccountReviewedToOneAsync(AdminOrderReviewedDto order);
}
