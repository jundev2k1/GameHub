namespace game_x.share.ExternalApi.Base;

public sealed class SecureRequest<T>
{
    public T Data { get; init; } = default!;
    public string Signature { get; init; } = null!;
}
