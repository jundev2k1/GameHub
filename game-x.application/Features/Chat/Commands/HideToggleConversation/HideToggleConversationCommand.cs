using System.Text.Json.Serialization;

namespace game_x.application.Features.Chat.Commands.HideToggleConversation;

public class HideToggleConversationCommand: ICommand<Unit>
{
    [JsonIgnore]
    public Guid ConversationId { get; set; }
    public bool IsHidden { get; set; }
}