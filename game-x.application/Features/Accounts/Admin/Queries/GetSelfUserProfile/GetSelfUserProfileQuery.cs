namespace game_x.application.Features.Accounts.Admin.Queries.GetSelfUserProfile;

public record GetSelfUserProfileQuery : IQuery<GetSelfUserProfileResult>;

public record GetSelfUserProfileResult(
    string UserId,
    string Username,
    string Email,
    string Nickname,
    string[] Roles);
