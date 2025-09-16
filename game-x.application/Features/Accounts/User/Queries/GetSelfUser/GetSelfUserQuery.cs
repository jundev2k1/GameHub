namespace game_x.application.Features.Accounts.User.Queries.GetSelfUser;

public record GetSelfUserQuery : IQuery<GetSelfUserResult>;

public record GetSelfUserInternalInfo(
    Guid WalletId,
    decimal Amount,
    decimal FrozenAmount,
    decimal TotalAmount,
    NetworkType Network,
    string Symbol);

public record GetSelfUserExternalInfo(
    Guid PlatformId,
    string PlatformName,
    decimal Amount);

public record GetSelfUserResult(
    string UserId,
    string Email,
    string Nickname,
    GetSelfUserInternalInfo[] InternalWallets,
    GetSelfUserExternalInfo[] ExternalWallets,
    bool IsEmailConfirmed,
    bool IsKycConfirmed,
    bool IsBankConfirmed,
    string[] Roles,
    string? AvatarUrl = null);
