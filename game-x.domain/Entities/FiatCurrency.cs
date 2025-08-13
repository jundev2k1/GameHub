namespace game_x.domain.Entities;

public sealed class FiatCurrency : BaseEntity<int>, IAuditable
{
    public Guid PublicId { get; private set; } = Guid.NewGuid();
    public CurrencyUnit Code { get; private set; } = default!;
    public string Name { get; private set; } = string.Empty;
    public string Symbol { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;

    public static FiatCurrency Create(
        CurrencyUnit code,
        string name,
        string symbol,
        string desc,
        bool isActive = true)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or empty.", nameof(name));
        if (string.IsNullOrWhiteSpace(symbol))
            throw new ArgumentException("Symbol cannot be null or empty.", nameof(symbol));

        return new FiatCurrency()
        {
            Code = code,
            Name = name,
            Symbol = symbol,
            Description = desc,
            IsActive = isActive
        };
    }
}
