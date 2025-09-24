using game_x.application.Features.LiveStreams.Streaming.Dtos;

namespace game_x.application.Features.LiveStreams.Streaming.Mapping;

public sealed class MapsterConfig : IRegister
{
    public void Register(TypeAdapterConfig cfg)
    {
        cfg.NewConfig<LiveStreamChatMessage, LiveStreamChatMessageDto>()
            .Map(dest => dest.Id, src => src.PublicId)
            .Map(dest => dest.StreamId, src => src.LiveStream.PublicId)
            .Map(dest => dest.StreamKey, src => src.LiveStream.StreamKey)
            .Map(dest => dest.NickName, src => src.Sender.Nickname)
            .Map(dest => dest.IsHost, src => src.SenderId == src.LiveStream.AssignedId);
    }
}
