namespace game_x.application.Features.ChainTransactions.Shared.Queries.GetCryptoTokenList;

public record GetCryptoTokenListQuery: IQuery<IEnumerable<CryptoTokenDto>>;

public record CryptoTokenDto(
    Guid Id,
    string Symbol,
    NetworkType Network,
    string ContractAddress,
    CryptoTokenStatus Status);