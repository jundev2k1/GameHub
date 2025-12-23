using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.Transactions.Shared.Queries.GetCryptoTokenList;

public sealed class GetCryptoTokenListHandler(ICryptoTokenRepo cryptoTokenRepo)
    : IQueryHandler<GetCryptoTokenListQuery, IEnumerable<CryptoTokenDto>>
{
    public async Task< IEnumerable<CryptoTokenDto>> Handle(GetCryptoTokenListQuery request, CancellationToken ct = default)
    {
        var cryptoTokens = await cryptoTokenRepo.GetCryptoTokenListAsync(ct);
        return cryptoTokens.Select(item => item.Adapt<CryptoTokenDto>());
    }
}