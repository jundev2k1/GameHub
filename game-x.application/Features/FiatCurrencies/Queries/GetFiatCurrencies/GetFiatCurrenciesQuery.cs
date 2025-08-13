using game_x.application.Features.FiatCurrencies.Dtos;

namespace game_x.application.Features.FiatCurrencies.Queries.GetFiatCurrencies;

public record GetFiatCurrenciesQuery : IQuery<FiatCurrencyDto[]>;
