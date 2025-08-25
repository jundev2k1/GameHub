
namespace game_x.application.Features.Accounts.Admin.Queries.GetAllActiveTokensByAdmin;

public record GetUserActiveTokensByAdminQuery(string UserId) : IQuery<GetAllActiveTokensDto[]>;

public sealed record GetUserActiveTokensDto(
    Guid PublicId,
    string IpAddress,
    string UserAgent,
    string DeviceInfo,
    DateTime CreatedAt);
