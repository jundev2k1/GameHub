using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace game_x.infrastructure.Security.Asymmetric;

public static class JsonSignatureNormalizer
{
    public static byte[] ComputeHashToByte(object data)
    {
        var normalizedData = NormalizeAndSerialize(data);
        var jsonString = JsonSerializer.Serialize(normalizedData);
        var inputStringByte = Encoding.UTF8.GetBytes(jsonString);
        var hashBytes = SHA256.HashData(inputStringByte);
        return hashBytes;
    }

    public static object NormalizeAndSerialize(object data)
    {
        if (data is string) return data;

        // 將物件轉成 Dictionary<string, object>
        var dictionary = ConvertToDictionary(data);

        // 將鍵轉換為駝峰命名並排序
        var camelAndSorted = dictionary
            .ToDictionary(
                kv => ToCamelCase(kv.Key),
                kv => kv.Value)
            .OrderBy(kv => kv.Key)  // ASCII 排序鍵
            .ToDictionary(kv => kv.Key, kv => kv.Value);

        return camelAndSorted;
    }
    
    private static string ToCamelCase(string input)
    {
        if (string.IsNullOrEmpty(input) || input.Length < 2) return input;
        return char.ToLowerInvariant(input[0]) + input.Substring(1);
    }

    private static Dictionary<string, object?> ConvertToDictionary(object obj)
    {
        return obj.GetType().GetProperties()
            .ToDictionary(p => p.Name, p => p.GetValue(obj));
    }
}
