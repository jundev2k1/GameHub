using game_x.application.Features.LiveStreams.Streaming.Dtos;

namespace game_x.application.Features.LiveStreams.Streaming.Queries.GetChatMessageInStream;

public record GetChatMessageInStreamQuery(
    string StreamKey,
    Guid MessageId,
    bool IsNext,
    int PageSize) : IQuery<LiveStreamChatMessageDto[]>;
