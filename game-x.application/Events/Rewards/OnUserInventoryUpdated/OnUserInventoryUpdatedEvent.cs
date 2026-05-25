using game_x.application.Features.Rewards.Dtos;

namespace game_x.application.Events.Rewards.OnUserInventoryUpdated;

public sealed record OnUserInventoryUpdatedEvent(string UserId, UserInventoryDto[] Dto) : IApplicationEvent;