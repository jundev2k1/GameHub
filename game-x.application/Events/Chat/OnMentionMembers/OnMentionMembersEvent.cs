using game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;

namespace game_x.application.Events.Chat.OnMentionMembers;

public record OnMentionMembersEvent(CreatedMessageSignalResult Res) : IApplicationEvent;