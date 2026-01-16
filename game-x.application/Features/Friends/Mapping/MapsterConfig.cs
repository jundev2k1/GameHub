using game_x.application.Contract.Infrastructure.SignalR.Dtos.Friend;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Notification;
using game_x.application.Features.Accounts.Dtos;
using game_x.application.Features.Friends.Dtos;

namespace game_x.application.Features.Friends.Mapping;

public sealed class MapsterConfig : IRegister
{
    public void Register(TypeAdapterConfig cfg)
    {
        cfg.NewConfig<UserDto, FriendSearchResultDto>()
            .Map(dest => dest.AvatarUrl, src => ""); 
        
        cfg.NewConfig<SocialLink, SocialLinkDto>()
            .Map(dest => dest.LinkId, src => src.PublicId)
            .Map(dest => dest.AddresseeNickname, src => src.AddresseeUser!.Nickname)
            .Map(dest => dest.AddresseeAvatarUrl, src => string.Empty)
            .Map(dest => dest.RequesterNickname, src => src.RequesterUser!.Nickname)
            .Map(dest => dest.RequesterAvatarUrl, src => string.Empty)
            .Map(dest => dest.BlockerNickname, src => src.BlockerUser!.Nickname)
            .Map(dest => dest.BlockerAvatarUrl, src => string.Empty)
            .Map(dest => dest.BlockedNickname, src => src.BlockedUser!.Nickname)
            .Map(dest => dest.BlockedAvatarUrl, src => string.Empty);
        
        cfg.NewConfig<FriendResponseSignalDto, FriendResponseNotificationDto>()
            .Map(dest => dest.Id, src => src.LinkId); 
    }
}