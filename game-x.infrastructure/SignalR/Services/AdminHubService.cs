using game_x.application.Common.Abstractions;
using game_x.application.Contract.Infrastructure.SignalR.Dtos;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Notification;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Transactions;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Features.BankAccountVerifications.Dtos;
using game_x.application.Features.Kyc.Dtos;
using game_x.infrastructure.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace game_x.infrastructure.SignalR.Services;

public sealed class AdminHubService(IHubContext<AdminHub, IAdminHub> hubContext)
    : IAdminHubService, IHubServices
{
    public async Task SendNotificationAsync(string adminId, NotificationDto message)
    {
        await hubContext.Clients.Group($"admin-{adminId}").ReceiveNotification(message);
    }

    public async Task SendNotificationToAllAsync(NotificationDto message)
    {
        await hubContext.Clients.All.ReceiveNotification(message);
    }

    public async Task SendTransactionToAdminAsync(string adminId, AdminTransactionDto transaction)
    {
        await hubContext.Clients.Group($"admin-{adminId}").TransactionUpdated(transaction);
    }

    public async Task SendVerificationToAdminAsync(string adminId, UserKycListItemDto verification)
    {
        await hubContext.Clients.Group($"admin-{adminId}").KycCreated(verification);
    }

    public async Task SendVerificationToAdminAsync(string adminId, BankAccountListItemDto verification)
    {
        await hubContext.Clients.Group($"admin-{adminId}").BankAccountCreated(verification);
    }

    public async Task NotifyOrderTxReviewedToAdminAsync(AdminOrderReviewedDto order)
    {
        await hubContext.Clients.Group("admin-group").TransactionReviewed(order);
    }

    public async Task NotifyOrderKycReviewedToAdminAsync(AdminOrderReviewedDto order)
    {
        await hubContext.Clients.Group("admin-group").KycReviewed(order);
    }

    public async Task NotifyOrderBankAccountReviewedToAdminAsync(AdminOrderReviewedDto order)
    {
        await hubContext.Clients.Group("admin-group").BankAccountReviewed(order);
    }
}
