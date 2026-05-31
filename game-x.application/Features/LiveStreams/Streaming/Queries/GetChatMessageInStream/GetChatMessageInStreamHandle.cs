using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Features.LiveStreams.Streaming.Dtos;

namespace game_x.application.Features.LiveStreams.Streaming.Queries.GetChatMessageInStream;

public sealed class GetChatMessageInStreamHandle(
    ILiveStreamManagerCacheService liveStreamManager)
    : IQueryHandler<GetChatMessageInStreamQuery, LiveStreamChatMessageDto[]>
{
    public async Task<LiveStreamChatMessageDto[]> Handle(GetChatMessageInStreamQuery request, CancellationToken ct = default)
    {
        var result = liveStreamManager.GetAdjacentMessages(
            request.StreamKey!, request.MessageId, request.IsNext, request.PageSize);
        return await Task.FromResult(result);
    }
}
