using game_x.application.Contract.Infrastructure.SignalR.Dtos;

namespace game_x.application.Contract.Infrastructure.SignalR.Services;

public interface IStoreHubService
{
    Task SendOrderStatusToStaffAsync(string staffId, StaffOrderStatusDto orderInfo);
}
