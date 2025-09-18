using game_x.application.Features.Accounts.Dtos;
using System.Text.Json.Serialization;

namespace game_x.application.Features.LiveStreams.Commands.AssignTalent;

public record AssignTalentCommand(
    [property: JsonIgnore] Guid Id,
    string TalentId) : ICommand<UserSummaryInfo>;
