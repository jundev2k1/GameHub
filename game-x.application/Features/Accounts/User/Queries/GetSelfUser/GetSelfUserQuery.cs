namespace game_x.application.Features.Accounts.User.Queries.GetSelfUser;

public record GetSelfUserQuery : IQuery<GetSelfUserResult>;

public record GetSelfUserResult(
    string UserId,
    string Email,
    string Nickname,
    UserStatus Status,
    bool IsKycConfirmed,
    bool IsBankConfirmed,
    string[] Roles);
