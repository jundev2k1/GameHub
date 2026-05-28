namespace game_x.application.Features.Rewards.Commands.Missions.GenerateShareLink;

public sealed record ShareLinkCommand(Guid MissionId) : ICommand<ShareLinkResponse>;

public sealed record ShareLinkResponse(string Url);