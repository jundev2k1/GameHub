using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.FiatCurrencies.Dtos;

namespace game_x.application.Features.FiatCurrencies.Queries.GetFiatCurrencies;

public sealed class GetFiatCurrenciesHandler(IFiatCurrencyRepo currencyRepo) : IQueryHandler<GetFiatCurrenciesQuery, FiatCurrencyDto[]>
{
    public async Task<FiatCurrencyDto[]> Handle(GetFiatCurrenciesQuery request, CancellationToken ct = default)
    {
        var currencyList = await currencyRepo.GetAllAsync(ct);
        var result = currencyList
            .Select(fc => fc.Adapt<FiatCurrencyDto>())
            .ToArray();
        return result;
    }
}
