using game_x.application.Features.Rewards.Dtos;
using game_x.domain.Enum.Rewards;

namespace game_x.application.Features.Rewards.Queries.Missions.GetListForUser;

public sealed record GetMissionListForUserQuery(MissionType? Type) : IQuery<IReadOnlyList<MissionUserDto>>;