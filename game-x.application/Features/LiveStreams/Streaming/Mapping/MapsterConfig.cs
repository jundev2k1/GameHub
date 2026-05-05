using game_x.application.Features.LiveStreams.Streaming.Dtos;

namespace game_x.application.Features.LiveStreams.Streaming.Mapping;

public sealed class MapsterConfig : IRegister
{
    public void Register(TypeAdapterConfig cfg)
    {
        cfg.NewConfig<LiveStreamChatMessage, LiveStreamChatMessageDto>()
            .Map(dest => dest.Id, src => src.PublicId)
            .Map(dest => dest.StreamId, src => src.LiveStream.PublicId)
            .Map(dest => dest.IsHost, src => src.SenderId == src.LiveStream.AssignedId)
            .Map(dest => dest.Nickname, src => src.Sender.Nickname);

        cfg.NewConfig<LiveStreamDonation, LiveStreamDonationDto>()
            .Map(dest => dest.Id, src => src.PublicId)
            .Map(dest => dest.LivestreamScheduleId, src => src.LivestreamSchedule.PublicId)
            .Map(dest => dest.DonorId, src => src.DonorId)
            .Map(dest => dest.DonorName, src => src.Donor.Nickname)
            .Map(dest => dest.GiftId, src => src.Gift != null ? src.Gift.PublicId : (Guid?)null)
            .Map(dest => dest.Animation, src => src.Gift != null ? src.Gift.Animation : null)
            .Map(dest => dest.AnimationDuration, src => src.Gift != null ? src.Gift.AnimationDuration : null);
    }
}
