using game_x.application.Contract.Infrastructure.SignalR.Dtos;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Notification;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Transactions;
using game_x.application.Features.BankAccountVerifications.Dtos;
using game_x.application.Features.Kyc.Dtos;
using Microsoft.EntityFrameworkCore;

namespace game_x.application.Contract.Infrastructure.SignalR.Services;

public interface IAdminHubService
{
    Task SendNotificationAsync(string adminId, NotificationDto message);

    Task SendNotificationToAllAsync(NotificationDto message);

    Task SendTransactionToAdminAsync(string adminId, AdminTransactionDto transaction);

    Task SendVerificationToAdminAsync(string adminId, UserKycListItemDto message);

    Task SendVerificationToAdminAsync(string adminId, BankAccountListItemDto message);

    Task NotifyOrderTxReviewedToAdminAsync(AdminOrderReviewedDto order);

    Task NotifyOrderKycReviewedToAdminAsync(AdminOrderReviewedDto order);

    Task NotifyOrderBankAccountReviewedToAdminAsync(AdminOrderReviewedDto order);
}
