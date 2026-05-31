using game_x.application.Features.Accounts.User.Dtos;

namespace game_x.application.Features.Accounts.User.Queries.GetSelfVerificationStatusList;

public record GetSelfVerificationStatusListQuery() : IQuery<VerificationStatusDto[]>;
