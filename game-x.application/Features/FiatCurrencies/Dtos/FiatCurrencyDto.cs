namespace game_x.application.Features.FiatCurrencies.Dtos;

public sealed class FiatCurrencyDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
}
