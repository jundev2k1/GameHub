using game_x.application.Features.Accounts.Dtos;
using game_x.application.Features.Accounts.User.Dtos;

namespace game_x.application.Features.Accounts.Admin.Queries.GetUserDetailByAdmin;

public record GetUserDetailByAdminQuery(string UserId) : IQuery<GetUserDetailByAdminResult>;

public record GetUserDetailByAdminResult(
    string UserId,
    string Username,
    string MemberNumber,
    string Email,
    string Nickname,
    string FullName,
    string AvatarUrl,
    DateTime? DateOfBirth,
    string? ResidentialAddress,
    bool IsEmailConfirmed,
    bool IsKycConfirmed,
    bool IsBankConfirmed,
    BalanceInfo[] InternalBalances,
    decimal TotalBalance,
    UserWalletExternalItemDto[] ExternalBalances,
    decimal TotalGamePoint,
    decimal TotalAsset,
    UserExtendDto UserExtendInfo,
    string[] Roles,
    DateTime? CreatedAt,
    DateTime? UpdatedAt);
