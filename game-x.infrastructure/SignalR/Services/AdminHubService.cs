using game_x.application.Common.Abstractions;
using game_x.application.Contract.Infrastructure.SignalR.Dtos;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Notification;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Transactions;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Features.BankAccountVerifications.Dtos;
using game_x.application.Features.Kyc.Dtos;
using game_x.infrastructure.SignalR.Facade;
using game_x.infrastructure.SignalR.Hubs;

namespace game_x.infrastructure.SignalR.Services;

public sealed class AdminHubService(ActorHubFacade<AdminHub, IAdminHub> actorHub) : IAdminHubService, IHubServices
{
    public async Task SendNotificationAsync(string adminId, NotificationDto message)
    {
        await actorHub.Admin(adminId).ReceiveNotification(message);
    }

    public async Task SendNotificationToAllAsync(NotificationDto message)
    {
        await actorHub.All().ReceiveNotification(message);
    }

    public async Task SendTransactionToAdminAsync(string adminId, AdminTransactionDto transaction)
    {
        await actorHub.Admin(adminId).TransactionUpdated(transaction);
    }

    public async Task SendVerificationToAdminAsync(string adminId, UserKycListItemDto verification)
    {
        await actorHub.Admin(adminId).KycCreated(verification);
    }

    public async Task SendVerificationToAdminAsync(string adminId, BankAccountListItemDto verification)
    {
        await actorHub.Admin(adminId).BankAccountCreated(verification);
    }

    public async Task NotifyOrderTxReviewedToAdminAsync(AdminOrderReviewedDto order)
    {
        await actorHub.AdminAll().TransactionReviewed(order);
    }

    public async Task NotifyOrderKycReviewedToAdminAsync(AdminOrderReviewedDto order)
    {
        await actorHub.AdminAll().KycReviewed(order);
    }

    public async Task NotifyOrderBankAccountReviewedToAdminAsync(AdminOrderReviewedDto order)
    {
        await actorHub.AdminAll().BankAccountReviewed(order);
    }
}