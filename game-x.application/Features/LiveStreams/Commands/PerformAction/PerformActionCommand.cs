using game_x.application.Features.LiveStreams.Enum;
using System.Text.Json.Serialization;

namespace game_x.application.Features.LiveStreams.Commands.PerformAction;

public record PerformActionCommand(
    [property: JsonIgnore] string? StreamKey,
    string ViewerId,
    PerformActionEnum Action,
    int? BlockTime,
    BlockReasonEnum? Reason) : ICommand;
