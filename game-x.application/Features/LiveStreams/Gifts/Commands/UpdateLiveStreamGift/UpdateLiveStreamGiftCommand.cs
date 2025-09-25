using System.Text.Json.Serialization;

namespace game_x.application.Features.LiveStreams.Gifts.Commands.UpdateLiveStreamGift;

public record UpdateLiveStreamGiftCommand(
    [property: JsonIgnore] Guid Id,
    string Name,
    string? Notes,
    int Priority) : ICommand;
