using System.Text.Json.Serialization;

namespace game_x.application.Features.LiveStreams.Commands.UpdateCategoryStatus;

public record UpdateCategoryStatusCommand(
    [property: JsonIgnore] Guid Id,
    bool IsActive) : ICommand;
