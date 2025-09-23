using game_x.application.Features.LiveStreams.Dtos;

namespace game_x.application.Features.LiveStreams.Queries.GetChatMessageInStream;

public record GetChatMessageInStreamQuery(
    string StreamKey,
    Guid MessageId,
    bool IsNext,
    int PageSize) : IQuery<LiveStreamChatMessageDto[]>;
