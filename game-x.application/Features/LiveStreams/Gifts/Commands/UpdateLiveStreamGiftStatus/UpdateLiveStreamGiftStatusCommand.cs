using System.Text.Json.Serialization;

namespace game_x.application.Features.LiveStreams.Gifts.Commands.UpdateLiveStreamGiftStatus;

public record UpdateLiveStreamGiftStatusCommand(
    [property: JsonIgnore] Guid Id,
    bool IsActive) : ICommand;
