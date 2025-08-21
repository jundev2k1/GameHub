using game_x.application.Features.Auth.Dtos;

namespace game_x.application.Features.Accounts.User.Queries.GetAllActiveTokens;

public record GetAllActiveTokensQuery : IQuery<GetAllActiveTokensDto[]>;

public record GetAllActiveTokensDto(
    Guid PublicId,
    string IpAddress,
    string UserAgent,
    string DeviceInfo,
    DateTime CreatedAt);
