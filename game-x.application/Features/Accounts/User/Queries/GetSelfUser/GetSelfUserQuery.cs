using game_x.application.Features.Accounts.Dtos;

namespace game_x.application.Features.Accounts.User.Queries.GetSelfUser;

public record GetSelfUserQuery : IQuery<GetSelfUserResult>;

public record GetSelfUserCyptoInfo(
    decimal Amount,
    decimal FrozenAmount,
    decimal TotalAmount,
    NetworkType Network,
    string Symbol);

public record GetSelfUserResult(
    string UserId,
    string Email,
    string Nickname,
    GetSelfUserCyptoInfo[] SiteBalances,
    decimal? GameBalance,
    bool IsEmailConfirmed,
    bool IsKycConfirmed,
    bool IsBankConfirmed,
    string[] Roles);
