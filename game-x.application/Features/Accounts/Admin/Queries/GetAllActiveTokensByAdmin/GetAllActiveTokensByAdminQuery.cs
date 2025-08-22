
namespace game_x.application.Features.Accounts.Admin.Queries.GetAllActiveTokensByAdmin;

public record GetAllActiveTokensByAdminQuery(string UserId) : IQuery<GetAllActiveTokensDto[]>;

public sealed record GetAllActiveTokensDto(
    Guid PublicId,
    string IpAddress,
    string UserAgent,
    string DeviceInfo,
    DateTime CreatedAt);
