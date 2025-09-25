using System.Text.Json.Serialization;

namespace game_x.application.Features.Friends.Commands.RespondFriendRequest;
    
public class RespondFriendRequestCommand: IRequest<Unit>
{
    [JsonIgnore]
    public Guid LinkPublicId { get; set; }
    public bool Accept { get; set; }
}