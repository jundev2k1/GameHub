using game_x.application.Features.Rewards.Dtos;

namespace game_x.application.Features.Rewards.Queries.Missions.GetByUser;

public record GetMissionDetailByUserQuery(Guid Id) : IQuery<MissionUserDto>;