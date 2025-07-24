using game_x.share.Attributes;
using System.Reflection;

namespace game_x.share.Extensions;

public static class EnumExtension
{
    public static string? ToMessage(this Enum enumValue)
    {
        var member = enumValue.GetType()
            .GetMember(enumValue.ToString())
            .FirstOrDefault();

        var message = member?.GetCustomAttribute<EnumMetadata>()
            ?.Message;
        return message;
    }

    public static int ToHttpStatus(this Enum enumValue)
    {
        var member = enumValue.GetType()
            .GetMember(enumValue.ToString())
            .FirstOrDefault();

        var status = member?.GetCustomAttribute<EnumMetadata>()
            ?.HttpStatus
            ?? 400;
        return status;
    }
}
