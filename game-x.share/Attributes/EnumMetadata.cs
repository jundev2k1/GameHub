namespace game_x.share.Attributes;

public sealed class EnumMetadata(string message, int httpStatus = 400) : Attribute
{
    public string Message { get; } = message;
    public int HttpStatus { get; } = httpStatus;
}
