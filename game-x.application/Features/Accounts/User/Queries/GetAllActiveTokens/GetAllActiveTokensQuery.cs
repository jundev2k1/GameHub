namespace game_x.application.Features.Accounts.User.Queries.GetAllActiveTokens;

public record GetAllActiveTokensQuery : IQuery<GetAllActiveTokensDto[]>;

public record GetAllActiveTokensDto(
    Guid PublicId,
    bool IsCurrentToken,
    string IpAddress,
    string UserAgent,
    string DeviceInfo,
    DateTime CreatedAt);
