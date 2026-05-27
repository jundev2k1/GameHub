using game_x.application.Features.Rewards.Dtos;

namespace game_x.application.Features.Rewards.Queries.Missions.GetDetailForUser;

public sealed record GetMissionDetailForUserQuery(Guid Id) : IQuery<MissionUserDto>;