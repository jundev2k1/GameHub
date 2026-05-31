using System.Text.Json;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Friend;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Notification;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Events.Friendship.OnRespondRequest;

public sealed class OnRespondRequestHandler(
    IUnitOfWork unitOfWork,
    IChatHubService chatHubService,
    IClientHubService clientHubService,
    INotificationRepo notificationRepo) : IApplicationEventHandler<OnRespondRequestEvent>
{
    public async Task Handle(OnRespondRequestEvent @event, CancellationToken ct = default)
    {
        var dto = @event.Dto.Adapt<FriendResponseSignalDto>();
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await SendNotificationToRequester(dto, @event.Conv, ct);
        }, ct);
    }
    
    private async Task SendNotificationToRequester(FriendResponseSignalDto dto, ConversationSignalDto conv, CancellationToken ct)
    {
        if (dto.RequesterUserId is not null)
        {
            if (dto.State == SocialLinkState.Accepted)
            {
                var metadata = JsonSerializer.Serialize(dto.Adapt<FriendResponseNotificationDto>());
                var notification = Notification.Create(
                    NotificationMessageKey.Friend_Request_Accepted,
                    dto.RequesterUserId,
                    NotificationType.SocialLink,
                    NotificationSeverity.Success,
                    metadata);
                await notificationRepo.AddNotificationAsync(notification, ct);
                await clientHubService.SendNotificationToMemberAsync(
                    dto.RequesterUserId,
                    notification.Adapt<NotificationDto>());
            }
            
            await chatHubService.SendFriendResponseAsync(dto, conv);
        }
    }
}