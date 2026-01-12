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

public sealed class CsAdminHubService(ActorHubFacade<CsAdminHub, ICsAdminHub> actorHub)
    : ICsAdminHubService, IHubServices
{
    public async Task SendNotificationToOneAsync(string userId, NotificationDto message)
    {
        await actorHub.Cs(userId).ReceiveNotification(message);
    }

    public async Task SendNotificationToAllAsync(NotificationDto message)
    {
        await actorHub.All().ReceiveNotification(message);
    }
    
    public async Task SendTransactionToOneAsync(string userId, AdminTransactionDto transaction)
    {
        await actorHub.Cs(userId).TransactionUpdated(transaction);
    }

    public async Task SendVerificationToOneAsync(string userId, UserKycListItemDto verification)
    {
        await actorHub.Cs(userId).KycCreated(verification);
    }

    public async Task SendVerificationToOneAsync(string userId, BankAccountListItemDto verification)
    {
        await actorHub.Cs(userId).BankAccountCreated(verification);
    }

    public async Task NotifyOrderTxReviewedToOneAsync(AdminOrderReviewedDto order)
    {
        await actorHub.CsAll().TransactionReviewed(order);
    }

    public async Task NotifyOrderKycReviewedToOneAsync(AdminOrderReviewedDto order)
    {
        await actorHub.CsAll().KycReviewed(order);
    }

    public async Task NotifyOrderBankAccountReviewedToOneAsync(AdminOrderReviewedDto order)
    {
        await actorHub.CsAll().BankAccountReviewed(order);
    }
}