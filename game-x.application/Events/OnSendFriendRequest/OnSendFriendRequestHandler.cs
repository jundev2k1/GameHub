using System.Text.Json;
using game_x.application.Contract.Infrastructure.SignalR.Dtos;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Friend;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Notification;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Events.OnSendFriendRequest;

public sealed class OnSendFriendRequestHandler(
    IUnitOfWork unitOfWork,
    IChatHubService chatHubService,
    IClientHubService clientHubService,
    INotificationRepo notificationRepo
    )
    : IApplicationEventHandler<OnSendFriendRequestEvent>
{
    public async Task Handle(OnSendFriendRequestEvent @event, CancellationToken ct = default)
    {
        var dto = @event.Dto.Adapt<FriendRequestSignalDto>();
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await SendNotificationToAddressee(dto, ct);
        }, ct);
    }
    
    private async Task SendNotificationToAddressee(FriendRequestSignalDto dto, CancellationToken ct)
    {
        if (dto.AddresseeUserId is not null)
        {
            var metadata = JsonSerializer.Serialize(dto.Adapt<FriendRequestNotificationDto>());
            var notification = Notification.Create(
                NotificationMessageKey.Friend_Request_Created,
                dto.AddresseeUserId,
                NotificationType.SocialLink,
                NotificationSeverity.Success,
                metadata);
            await notificationRepo.AddNotificationAsync(notification, ct);

            await clientHubService.SendNotificationToMemberAsync(
                dto.AddresseeUserId,
                notification.Adapt<NotificationDto>());
            
            await chatHubService.SendFriendRequestAsync(dto);
        }
    }
}