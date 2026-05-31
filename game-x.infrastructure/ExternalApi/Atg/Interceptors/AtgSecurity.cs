using System.Security.Cryptography;
using System.Text;

namespace game_x.infrastructure.ExternalApi.Atg.Interceptors;

internal static class AtgSecurity
{
    /// <summary>
    /// Computes ETL998 "sign" using the provider's rule: MD5(UTF8(lower(dataBase64))) as lower-case hex.
    /// </summary>
    public static string ComputeSignProvider(string dataBase64)
    {
        var bytes = Encoding.UTF8.GetBytes(dataBase64.ToLowerInvariant());
        var hash = MD5.HashData(bytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    /// <summary>
    /// Encrypts a JSON string using DES-CBC (PKCS7), with Key=IV=ASCII(key8), and returns Base64 ciphertext.
    /// </summary>
    public static string DesEncryptToBase64(string plainText, string key8)
    {
        ValidateDesKey(key8);

        var keyBytes = Encoding.UTF8.GetBytes(key8);
        var inputBytes = Encoding.UTF8.GetBytes(plainText);

        using var des = DES.Create();
        des.Mode = CipherMode.CBC;
        des.Padding = PaddingMode.PKCS7;
        des.Key = keyBytes;
        des.IV = keyBytes;

        using var ms = new MemoryStream();
        using (var cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
        {
            cs.Write(inputBytes, 0, inputBytes.Length);
            cs.FlushFinalBlock();
        }

        return Convert.ToBase64String(ms.ToArray());
    }

    /// <summary>
    /// Validates the DES key format required by ETL998: exactly 8 ASCII characters.
    /// </summary>
    private static void ValidateDesKey(string key8)
    {
        if (string.IsNullOrWhiteSpace(key8) || key8.Length != 8)
            throw new ArgumentException("DES key must be exactly 8 ASCII characters.", nameof(key8));
    }
}