using game_x.application.Features.LiveStreams.Dtos;
using System.Text.Json.Serialization;

namespace game_x.application.Features.LiveStreams.Queries.GetChatMessageInStream;

public record GetChatMessageInStreamQuery(
    [property: JsonIgnore] string? StreamKey,
    Guid MessageId,
    bool IsNext = false,
    int PageSize = 20) : IQuery<LiveStreamChatMessageDto[]>;
