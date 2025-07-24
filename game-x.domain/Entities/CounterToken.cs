namespace game_x.domain.Entities;

public sealed class CounterToken : BaseEntity<int>
{
    public int CounterId { get; set; }
    public string Token { get; private set; } = default!;
    public bool IsValid { get; private set; }
    public Counter Counter { get; private set; } = default!;

    public static CounterToken Create(
        string token,
        bool isValid = true)
    {
        var counterToken = new CounterToken
        {
            Token = token,
            IsValid = isValid,
        };
        return counterToken;
    }
}
