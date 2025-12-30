using System.Text.Json.Serialization;

namespace game_x.application.Features.Chat.Commands.ClaimConversationById;

public sealed record ClaimConversationByIdCommand : IRequest<Unit>
{
    [JsonIgnore]
    public Guid ConversationId {get; set;}
};