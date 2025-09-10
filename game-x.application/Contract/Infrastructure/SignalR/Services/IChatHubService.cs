using game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;

namespace game_x.application.Contract.Infrastructure.SignalR.Services;

public interface IChatHubService
{
    Task SendSupportMessageAsync(SendMessageResult res);
}